using MediaRecommender.Data.Contracts;
using MediaRecommender.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaRecommender.Data.TheMovieDb
{
    /// <summary>
    ///     Interface to an external provider of media recommendations.
    /// </summary>
    /// <seealso cref="MediaRecommender.Data.Contracts.IExternalMediaRecommendationProvider" />
    public class TheMovieDbProvider : IExternalMediaRecommendationProvider
    {
        #region Fields

        private readonly ITheMovieDbAdapter _theMovieDbAdapter;

        #endregion

        #region Constructor

        public TheMovieDbProvider(ITheMovieDbAdapter theMovieDbAdapter)
        {
            _theMovieDbAdapter = theMovieDbAdapter;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        ///     Gets a set of movies based on the provided parameter constraints.
        ///     Page limit is 20. 
        ///     Offset is 0.
        /// </summary>
        /// <param name="from">The start of the period when the movies have to be released.</param>
        /// <param name="to">The end of the period when the movies have to be released.</param>
        /// <param name="genres">Each movie must mach at least one of the provided genres.</param>
        /// <param name="keywordIds">Each movie must mach at least one of the provided internal provider keyword id.</param>
        /// <param name="page">The page on the expernal provider total number of results.</param>
        /// <returns>The set of movies that fulfill the provided constraints.</returns>
        public async Task<GetMoviesResponse> GetMoviesAsync(DateTime? from, DateTime? to, IEnumerable<Genre> genres, IEnumerable<string> keywordIds, int page)
            => await _theMovieDbAdapter.GetMoviesAsync(from, to, genres, keywordIds, page);

        /// <summary>
        ///     Gets the movie external provider keyword ids asynchronously.
        /// </summary>
        /// <param name="movieTitle">The movie title.</param>
        /// <param name="releaseYear">The release year.</param>
        /// <returns>The external provided keyword Ids related to the provided movie.</returns>
        public async Task<IEnumerable<string>> GetMovieExternalProviderKeywordIdsAsync(string movieTitle, int releaseYear)
            => await _theMovieDbAdapter.GetMovieExternalProviderKeywordIdsAsync(movieTitle, releaseYear);

        #endregion
    }
}
