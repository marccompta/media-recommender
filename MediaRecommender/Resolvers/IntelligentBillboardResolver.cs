using MediaRecommender.Contracts;
using MediaRecommender.Data.MoviesInternalDb;
using MediaRecommender.Data.MoviesInternalDb.Models;
using MediaRecommender.Entities;
using MediaRecommender.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("MediaRecommender.Unit.Tests")]
namespace MediaRecommender.Resolvers
{
    /// <summary>
    ///     Resolves the intelligent billboard.
    /// </summary>
    /// <seealso cref="IIntelligentBillboardResolver" />
    public class IntelligentBillboardResolver : IIntelligentBillboardResolver
    {
        #region Fields

        private readonly ILogger<IntelligentBillboardResolver> _logger;
        private readonly IBillboardRepository _billboardRepository;
        private readonly IInternalDbRepository _internalDbRepository;
        private readonly IGenresResolver _genresResolver;

        #endregion

        #region Constants

        private const int pageLimit = 20;
        private const int daysPerMonth = 30;
        private const int daysPerWeek = 7;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntelligentBillboardResolver"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="billboardRepository">The billboard repository.</param>
        /// <param name="internalDbRepository">The internal database repository.</param>
        /// <param name="genresResolver">The genres resolver.</param>
        public IntelligentBillboardResolver(ILogger<IntelligentBillboardResolver> logger, IBillboardRepository billboardRepository, IInternalDbRepository internalDbRepository,
            IGenresResolver genresResolver)
        {
            _logger = logger;
            _billboardRepository = billboardRepository;
            _internalDbRepository = internalDbRepository;
            _genresResolver = genresResolver;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        ///     Resolves the intelligent billboard based on a set of provided parameters asynchronously.
        /// </summary>
        /// <param name="from">The first day of the first week of the set of weeks for which the intelligent billboard is to be calculated.</param>
        /// <param name="numberOfWeeks">The number of weeks a billboard is to be calculated.</param>
        /// <param name="numberOfBigRooms">The number of big rooms.</param>
        /// <param name="numberOfSmallRooms">The number of small rooms.</param>
        /// <param name="city">The city location.</param>
        /// <returns>The intelligent billboard of recommendations for the set of weeks that corresponds to the provided parameters.</returns>
        public async Task<IEnumerable<IntelligentBillboardRecommendation>> ResolveAsync(DateTime from, int numberOfWeeks, int numberOfBigRooms, int numberOfSmallRooms, string city)
        {
            IEnumerable<string> keywordIds = await GetKeywordIdsOfSuccessfulMoviesInCity(city);

            var moviesAlreadyAssigned = new ConcurrentDictionary<string, byte>();
            var result = new List<IntelligentBillboardRecommendation>();
            var tasks = new List<Task>();

            for (int index = 0; index < numberOfWeeks; index++)
            {
                var intelligentBillboardRecommendation = new IntelligentBillboardRecommendation();
                intelligentBillboardRecommendation.WeekNumber = index + 1;
                result.Add(intelligentBillboardRecommendation);

                // We recommend movies that have been released during the last month.
                DateTime localFrom = from.AddDays(-daysPerMonth + (daysPerWeek * index));
                DateTime to = from.AddDays(daysPerWeek * index);

                tasks.Add(GenerateBillboardsAsync(localFrom, to, keywordIds, numberOfBigRooms, numberOfSmallRooms, moviesAlreadyAssigned, intelligentBillboardRecommendation));
            }

            await Task.WhenAll(tasks);
            
            return result;
        }

        #endregion

        #region Internal Methods

        internal async Task<IEnumerable<Recommendation>> GenerateBillboardAsync(DateTime from, DateTime to, IEnumerable<Genre> genres, IEnumerable<string> keywordIds, int targetQuantity, ConcurrentDictionary<string, byte> moviesAlreadyAssigned)
        {
            var result = new List<Recommendation>();
            int pageNum = 0;
            GetMoviesBillboardRepositoryResponse localResponse;

            do
            {
                pageNum++;
                localResponse = await _billboardRepository.GetMoviesAsync(from, to, genres, keywordIds, pageNum);

                if (localResponse?.Results != null && localResponse.Results.Any())
                {
                    var results = localResponse.Results.ToList();
                    int index = 0;
                    while (index < localResponse.Results.Count() && result.Count() < targetQuantity)
                    {
                        if (moviesAlreadyAssigned.TryAdd(results[index].Title, 0))
                        {
                            result.Add(results[index]);
                        }

                        index++;
                    }
                }
            } while (result.Count < targetQuantity
                && GetNumProcessedResults(pageNum, localResponse.Results.Count()) < localResponse.TotalResults
                && localResponse?.Results != null);

            return result;
        }

        #endregion

        #region Private Methods

        private async Task<IEnumerable<string>> GetKeywordIdsOfSuccessfulMoviesInCity(string city)
        {
            var result = Enumerable.Empty<string>();

            if (!string.IsNullOrWhiteSpace(city))
            {
                var successfulMovies = await _internalDbRepository.GetSuccessfulMoviesByCityName(city);
                if (successfulMovies != null)
                {
                    var keywordIds = await GetKeywordsAsync(successfulMovies);
                    if (keywordIds != null)
                    {
                        result = keywordIds;
                    }
                }
            }

            return result;
        }

        private async Task GenerateBillboardsAsync(DateTime from, DateTime to, IEnumerable<string> keywordIds, int numBigRooms, int numSmallRooms,
            ConcurrentDictionary<string, byte> moviesAlreadyAssigned, IntelligentBillboardRecommendation intelligentBillboardRecommendation)
        {
            // Generate billboard for the small rooms.
            var smallRoomsTask = GenerateBillboardAsync(from, to, _genresResolver.GetMinoritaryGenres(), keywordIds, numSmallRooms, moviesAlreadyAssigned);

            // Generate billboard for the big rooms.
            var bigRoomsTask = GenerateBillboardAsync(from, to, _genresResolver.GetBlockbusterGenres(), keywordIds, numBigRooms, moviesAlreadyAssigned);

            await Task.WhenAll(smallRoomsTask, bigRoomsTask);

            intelligentBillboardRecommendation.SmallRooms = await smallRoomsTask;
            intelligentBillboardRecommendation.BigRooms = await bigRoomsTask;
        }

        private int GetNumProcessedResults(int currentPage, int numResultsCurrentPage)
            => ((currentPage - 1) * pageLimit) + numResultsCurrentPage;

        private async Task<IEnumerable<string>> GetKeywordsAsync(IEnumerable<SuccessfulMovie> successfulMovies)
        {
            var result = new ConcurrentDictionary<string, byte>();

            if (successfulMovies != null && successfulMovies.Any())
            {
                List<Task> tasks = new List<Task>();

                foreach (var movie in successfulMovies)
                {
                    tasks.Add(RegisterNewKeywordsAsync(movie.Title, movie.ReleaseDate.Year, result));
                }

                await Task.WhenAll(tasks);
            }

            return result.Keys;
        }

        private async Task RegisterNewKeywordsAsync(string movieTitle, int releaseYear, ConcurrentDictionary<string, byte> keywordsStorage)
        {
            var response = await _billboardRepository.GetMovieExternalProviderKeywordIdsAsync(movieTitle, releaseYear);

            if (response != null)
            {
                foreach (var value in response)
                {
                    if (value != null)
                    {
                        keywordsStorage.TryAdd(value, 0);
                    }
                }
            }
        }

        #endregion
    }
}
