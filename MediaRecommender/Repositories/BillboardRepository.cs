using MediaRecommender.Contracts;
using MediaRecommender.Data.Contracts;
using MediaRecommender.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaRecommender.Repositories
{
    /// <summary>
    ///     The repository to retrieve billboard-related data from a source.
    /// </summary>
    /// <seealso cref="IBillboardRepository" />
    public class BillboardRepository : IBillboardRepository
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IExternalMediaRecommendationProvider _externalMediaRecommendationProvider;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="BillboardRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="externalMediaRecommendationProvider">The external media recommendation provider.</param>
        public BillboardRepository(IMapper mapper, IExternalMediaRecommendationProvider externalMediaRecommendationProvider)
        {
            _mapper = mapper;
            _externalMediaRecommendationProvider = externalMediaRecommendationProvider;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        ///     Gets the movie external provider keyword ids that correspond to the requested movie asynchronously.
        /// </summary>
        /// <param name="movieTitle">The movie title.</param>
        /// <param name="releaseYear">The release year.</param>
        /// <returns>Enumeration of source keyword ids for the movie.</returns>
        public async Task<IEnumerable<string>> GetMovieExternalProviderKeywordIdsAsync(string movieTitle, int releaseYear)
        {
            var keywordIds = await _externalMediaRecommendationProvider.GetMovieExternalProviderKeywordIdsAsync(movieTitle, releaseYear);

            return keywordIds;
        }

        /// <summary>
        ///     Gets movie recommendations asynchronously based on a set of filters.
        /// </summary>
        /// <param name="from">The initial value on the time period when the movies have to be released.</param>
        /// <param name="to">The end value on the time period when the movies have to be released.</param>
        /// <param name="genres">At least one of the provided genres must mach each of the retrieved movies.</param>
        /// <param name="keywordIds">At least one of the provided keyword Ids must match the ones of each of the provided movies.</param>
        /// <param name="page">The page number corresponding to this result set on the source pagination system.</param>
        /// <returns>The requestes movie recommendations.</returns>
        public async Task<GetMoviesBillboardRepositoryResponse> GetMoviesAsync(DateTime from, DateTime to, IEnumerable<Genre> genres, IEnumerable<string> keywordIds, int page)
        {
            GetMoviesBillboardRepositoryResponse result = null;
            var dataGenres = genres.Select(g => _mapper.Map(g));
            var response = await _externalMediaRecommendationProvider.GetMoviesAsync(from, to, dataGenres, keywordIds, page);

            if (response != null)
            {
                result = _mapper.Map(response);
            }

            return result;
        }

        #endregion
    }
}
