using MediaRecommender.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaRecommender.Data.Contracts
{
    /// <summary>
    ///     Interface to an external provider of media recommendations.
    /// </summary>
    public interface IExternalMediaRecommendationProvider
    {
        /// <summary>
        ///     Gets a set of movies based on the provided parameter constraints.
        /// </summary>
        /// <param name="from">The start of the period when the movies have to be released.</param>
        /// <param name="to">The end of the period when the movies have to be released.</param>
        /// <param name="genres">Each movie must mach at least one of the provided genres.</param>
        /// <param name="keywordIds">Each movie must mach at least one of the provided internal provider keyword id.</param>
        /// <param name="page">The page on the expernal provider total number of results.</param>
        /// <returns>The set of movies that fulfill the provided constraints.</returns>
        Task<GetMoviesResponse> GetMoviesAsync(DateTime? from, DateTime? to, IEnumerable<Genre> genres, IEnumerable<string> keywordIds, int page);

        /// <summary>
        ///     Gets the movie external provider keyword ids asynchronously.
        /// </summary>
        /// <param name="movieTitle">The movie title.</param>
        /// <param name="releaseYear">The release year.</param>
        /// <returns>The external provided keyword Ids related to the provided movie.</returns>
        Task<IEnumerable<string>> GetMovieExternalProviderKeywordIdsAsync(string movieTitle, int releaseYear);
    }
}
