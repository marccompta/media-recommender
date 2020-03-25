using MediaRecommender.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaRecommender.Data.TheMovieDb
{
    /// <summary>
    ///     Adapts the TheMovieDB API calls to calls used by the data layer.
    /// </summary>
    public interface ITheMovieDbAdapter
    {
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
        Task<GetMoviesResponse> GetMoviesAsync(DateTime? from, DateTime? to, IEnumerable<Genre> genres, IEnumerable<string> keywordIds, int page);

        /// <summary>
        ///     Gets the movie external provider keyword ids asynchronous.
        /// </summary>
        /// <param name="movieTitle">The movie title.</param>
        /// <param name="releaseYear">The release year.</param>
        /// <returns>Collection of TMDb internal keyword identifiers for the provided movie.</returns>
        Task<IEnumerable<string>> GetMovieExternalProviderKeywordIdsAsync(string movieTitle, int releaseYear);

        #endregion
    }
}
