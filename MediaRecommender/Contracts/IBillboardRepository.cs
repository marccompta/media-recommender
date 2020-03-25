using MediaRecommender.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaRecommender.Repositories
{
    /// <summary>
    ///     The repository to retrieve billboard-related data from a source.
    /// </summary>
    public interface IBillboardRepository
    {
        #region Public Methods

        /// <summary>
        ///     Gets movie recommendations asynchronously based on a set of filters.
        /// </summary>
        /// <param name="from">The initial value on the time period when the movies have to be released.</param>
        /// <param name="to">The end value on the time period when the movies have to be released.</param>
        /// <param name="genres">At least one of the provided genres must mach each of the retrieved movies.</param>
        /// <param name="keywordIds">At least one of the provided keyword Ids must match the ones of each of the provided movies.</param>
        /// <param name="page">The page number corresponding to this result set on the source pagination system.</param>
        /// <returns>The requestes movie recommendations.</returns>
        Task<GetMoviesBillboardRepositoryResponse> GetMoviesAsync(DateTime from, DateTime to, IEnumerable<Genre> genres, IEnumerable<string> keywordIds, int page);

        /// <summary>
        ///     Gets the movie external provider keyword ids that correspond to the requested movie asynchronously.
        /// </summary>
        /// <param name="movieTitle">The movie title.</param>
        /// <param name="releaseYear">The release year.</param>
        /// <returns>Enumeration of source keyword ids for the movie.</returns>
        Task<IEnumerable<string>> GetMovieExternalProviderKeywordIdsAsync(string movieTitle, int releaseYear);

        #endregion
    }
}
