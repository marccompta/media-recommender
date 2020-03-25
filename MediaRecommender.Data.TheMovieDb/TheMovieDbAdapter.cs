using MediaRecommender.Data.Models;
using MediaRecommender.Data.TheMovieDb.Mappers;
using MediaRecommender.Data.TheMovieDb.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaRecommender.Data.TheMovieDb
{
    /// <summary>
    ///     Adapts the TheMovieDB API calls to calls used by the data layer.
    /// </summary>
    /// <seealso cref="ITheMovieDbAdapter" />
    public class TheMovieDbAdapter : ITheMovieDbAdapter
    {
        #region Fields

        private readonly ILogger<TheMovieDbAdapter> _logger;
        private readonly ITheMovieDbGateway _theMovieDbGateway;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="TheMovieDbAdapter"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="theMovieDbGateway">The TMDb gateway.</param>
        /// <param name="mapper">The mapper.</param>
        public TheMovieDbAdapter(ILogger<TheMovieDbAdapter> logger, ITheMovieDbGateway theMovieDbGateway, IMapper mapper)
        {
            _logger = logger;
            _theMovieDbGateway = theMovieDbGateway;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the movies asynchronously.
        /// </summary>
        /// <param name="from">The start of the time period the movies have to be released within.</param>
        /// <param name="to">The end of the time period the movies have to be released within.</param>
        /// <param name="genres">Each movie must match at least a genre of this collection.</param>
        /// <param name="keywordIds">Each movie must match at least a keyword identifier of this collection.</param>
        /// <param name="page">The page that corresponds to the TMDb result set.</param>
        /// <returns>Movies that fulfill the provided parameter constraints.</returns>
        public async Task<GetMoviesResponse> GetMoviesAsync(DateTime? from, DateTime? to, IEnumerable<Data.Models.Genre> genres,
            IEnumerable<string> keywordIds, int page)
        {
            var genreTMDbIds = MapGenresToTMDbIds(genres);
            DiscoverMoviesResponse response = null;

            try
            {
                response = await _theMovieDbGateway.MakeDiscoverMoviesRequestAsync(from, to, genreTMDbIds, keywordIds, page);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error @GetMoviesAsync.MakeDiscoverMoviesRequestAsync [from={from}][to={to}][genreTMDbIds={genreTMDbIds}][keywordIds={keywordIds}][page={page}]");
            }
            

            if (response?.Results != null)
            {
                List<Task> tasks = new List<Task>();

                foreach (var result in response.Results)
                {
                    tasks.Add(EnrichMovie(result));
                }

                await Task.WhenAll(tasks);
            }

            return _mapper.Map(response);
        }

        /// <summary>
        ///     Gets the movie external provider keyword ids asynchronous.
        /// </summary>
        /// <param name="movieTitle">The movie title.</param>
        /// <param name="releaseYear">The release year.</param>
        /// <returns>Collection of TMDb internal keyword identifiers for the provided movie.</returns>
        public async Task<IEnumerable<string>> GetMovieExternalProviderKeywordIdsAsync(string movieTitle, int releaseYear)
        {
            SearchMovieResponse movie = null;

            try
            {
                movie = await _theMovieDbGateway.MakeSearchMovieRequestAsync(movieTitle, releaseYear);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error @GetMovieExternalProviderKeywordIdsAsync.MakeSearchMovieRequestAsync [movieTitle={movieTitle}][releaseYear={releaseYear}]");
            }

            if (movie?.Results != null && movie.Results.Any())
            {
                var movieId = movie.Results.First().Id;

                if (movieId > 0)
                {
                    GetMovieKeywordsResponse response = null;

                    try
                    {
                        response = await _theMovieDbGateway.MakeMovieKeywordsRequestAsync(movieId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error @GetMovieExternalProviderKeywordIdsAsync.MakeMovieKeywordsRequestAsync [movieId={movieId}]");
                    }

                    if (response?.Keywords != null && response.Keywords.Any())
                    {
                        return response.Keywords.Select(k => k.Id.ToString());
                    }
                    
                }
            }

            return Enumerable.Empty<string>();
        }

        #endregion

        #region PrivateMethods

        private async Task EnrichMovie(Models.Movie movie)
        {

            Models.Movie response = null;

            try
            {
                response = await _theMovieDbGateway.MakeMovieDetailRequestAsync(movie.Id, new[] { MovieDetailRequestIncludeTypes.Keywords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error @EnrichMovie.MakeMovieDetailRequestAsync [movieId={movie.Id}][blueprint=keywords]");
            }
            
            movie.KeywordsEnvelope = response?.KeywordsEnvelope ?? new GetMovieKeywordsResponse { Keywords = Enumerable.Empty<Keyword>() };
            movie.Homepage = response?.Homepage ?? string.Empty;
            movie.Genres = response?.Genres ?? Enumerable.Empty<Models.Genre>();
        }

        private IEnumerable<int> MapGenresToTMDbIds(IEnumerable<Data.Models.Genre> genres)
        {
            return genres?.Select(g => _mapper.Map(g).Id) ?? Enumerable.Empty<int>();
        }

        #endregion
    }
}
