using MediaRecommender.Data.TheMovieDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaRecommender.Data.TheMovieDb
{
    /// <summary>
    ///     Client of TheMovieDb API to retrieve media information.
    /// </summary>
    /// <seealso cref="ITheMovieDbGateway" />
    public class TheMovieDbGateway : ITheMovieDbGateway
    {
        #region Fields

        private readonly IApiClient _apiClient;

        #endregion

        #region Constants

        string baseUri = "api.themoviedb.org/3";

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="TheMovieDbGateway"/> class.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        public TheMovieDbGateway(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        ///     Makes a discover movies request asynchronously.
        /// </summary>
        /// <param name="releaseDateFrom">The start of the time period the movies have to be released within.</param>
        /// <param name="releaseDateTo">The end of the time period the movies have to be released within.</param>
        /// <param name="withGenres">Each movie must match at least a genre of this collection.</param>
        /// <param name="withKeywords">Each movie must match at least a keyword identifier of this collection.</param>
        /// <param name="page">The page that corresponds to the TMDb result set.</param>
        /// <returns>The TMDb Discover Movies corresponding response.</returns>
        public async Task<DiscoverMoviesResponse> MakeDiscoverMoviesRequestAsync(DateTime? releaseDateFrom, DateTime? releaseDateTo, IEnumerable<int> withGenres,
            IEnumerable<string> withKeywords, int page)
        {
            Uri uri = GetDiscoverMoviesUri(releaseDateFrom, releaseDateTo, withGenres, withKeywords, page);
            var result = await _apiClient.GetAsync<DiscoverMoviesResponse>(uri);

            return result;
        }

        /// <summary>
        ///     Makes a movie detail request asynchronously.
        /// </summary>
        /// <param name="movieId">The movie TMDb identifier.</param>
        /// <param name="blueprint">The blueprint.</param>
        /// <returns>The detail of the movie.</returns>
        public async Task<Movie> MakeMovieDetailRequestAsync(int movieId, IEnumerable<MovieDetailRequestIncludeTypes> blueprint)
        {
            Uri uri = GetMovieDetailUri(movieId, blueprint);
            var result = await _apiClient.GetAsync<Movie>(uri);

            return result;
        }

        /// <summary>
        ///     Makes a search movie request asynchronously.
        /// </summary>
        /// <param name="movieTitle">The movie title.</param>
        /// <param name="releaseYear">The release year.</param>
        /// <returns>A collection of possible matches.</returns>
        public async Task<SearchMovieResponse> MakeSearchMovieRequestAsync(string movieTitle, int releaseYear)
        {
            Uri uri = GetSearchMovieUri(movieTitle, releaseYear);
            var result = await _apiClient.GetAsync<SearchMovieResponse>(uri);

            return result;
        }

        /// <summary>
        ///     Makes a movie keywords request asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>The collection of TMDb movie keywords.</returns>
        public async Task<GetMovieKeywordsResponse> MakeMovieKeywordsRequestAsync(int movieId)
        {
            Uri uri = GetMovieKeywordsUri(movieId);
            var result = await _apiClient.GetAsync<GetMovieKeywordsResponse>(uri);

            return result;
        }

        #endregion

        #region Private Methods

        private Uri GetDiscoverMoviesUri(DateTime? releaseDateFrom, DateTime? releaseDateTo, IEnumerable<int> withGenres, IEnumerable<string> withKeywords, int page)
        {
            string releaseDateFromParam = releaseDateFrom.HasValue ? $"primary_release_date.gte={ releaseDateFrom.Value.ToString("yyyy-MM-dd") }&" : "";
            string releaseDateToParam = releaseDateTo.HasValue ? $"primary_release_date.lte={ releaseDateTo.Value.ToString("yyyy-MM-dd") }&" : "";
            string withGenresParam = withGenres != null && withGenres.Any() ? $"with_genres={ string.Join("|", withGenres) }&" : "";
            string keywordsParam = withKeywords != null && withKeywords.Any() ? $"with_keywords={ string.Join("|", withKeywords) }&" : "";
            string pageParam = "page=" + (page > 0 ? page : 1);

            string uri = $"https://{baseUri}/discover/movie?include_adult=false&sort_by=popularidy.desc&{releaseDateFromParam}{releaseDateToParam}{withGenresParam}{keywordsParam}{pageParam}";

            return new Uri(uri);
        }

        private Uri GetMovieDetailUri(int movieId, IEnumerable<MovieDetailRequestIncludeTypes> blueprint)
        {
            Uri result = null;
            string uri = $"https://{baseUri}/movie/{movieId}";

            if (blueprint != null && blueprint.Any())
            {
                string blueprintQueryParam = string.Join(",", blueprint.Select(b => Enum.GetName(typeof(MovieDetailRequestIncludeTypes), b).ToLower()));
                uri = $"{uri}?append_to_response={blueprintQueryParam}";

                result = new Uri(uri);
            }

            return result;
        }

        private Uri GetSearchMovieUri(string text, int releaseYear)
        {
            Uri result = null;

            if (!string.IsNullOrWhiteSpace(text) && releaseYear > 0)
            {
                string uri = $"https://{baseUri}/search/movie?query={text}&year={releaseYear}";
                result = new Uri(uri);
            }

            return result;
        }

        private Uri GetMovieKeywordsUri(int movieId)
        {
            Uri result = null;

            if (movieId > 0)
            {
                string uri = $"https://{baseUri}/movie/{movieId}/keywords";
                result = new Uri(uri);
            }

            return result;
        }

        #endregion
    }
}
