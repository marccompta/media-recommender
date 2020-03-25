using MediaRecommender.Data.TheMovieDb.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaRecommender.Data.TheMovieDb
{
    /// <summary>
    ///     Interface for a client of TheMovieDb API to retrieve media information.
    /// </summary>
    public interface ITheMovieDbGateway
    {
        #region Public Methods

        /// <summary>
        ///     Makes a discover movies request asynchronously.
        /// </summary>
        /// <param name="releaseDateFrom">The start of the time period the movies have to be released within.</param>
        /// <param name="releaseDateTo">The end of the time period the movies have to be released within.</param>
        /// <param name="withGenres">Each movie must match at least a genre of this collection.</param>
        /// <param name="withKeywords">Each movie must match at least a keyword identifier of this collection.</param>
        /// <param name="page">The page that corresponds to the TMDb result set.</param>
        /// <returns></returns>
        Task<DiscoverMoviesResponse> MakeDiscoverMoviesRequestAsync(DateTime? releaseDateFrom, DateTime? releaseDateTo,
            IEnumerable<int> withGenres, IEnumerable<string> withKeywords, int page);

        /// <summary>
        ///     Makes a movie detail request asynchronously.
        /// </summary>
        /// <param name="movieId">The movie TMDb identifier.</param>
        /// <param name="blueprint">The blueprint.</param>
        /// <returns>The detail of the movie.</returns>
        Task<Movie> MakeMovieDetailRequestAsync(int movieId, IEnumerable<MovieDetailRequestIncludeTypes> blueprint);

        /// <summary>
        ///     Makes a search movie request asynchronously.
        /// </summary>
        /// <param name="movieTitle">The movie title.</param>
        /// <param name="releaseYear">The release year.</param>
        /// <returns>A collection of possible matches.</returns>
        Task<SearchMovieResponse> MakeSearchMovieRequestAsync(string movieTitle, int releaseYear);

        /// <summary>
        ///     Makes a movie keywords request asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>The collection of TMDb movie keywords.</returns>
        Task<GetMovieKeywordsResponse> MakeMovieKeywordsRequestAsync(int movieId);

        #endregion
    }
}
