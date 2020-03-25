using MediaRecommender.Entities;
using System.Collections.Generic;

namespace MediaRecommender.Contracts
{
    /// <summary>
    ///     Resolves the genres that correspond to each category.
    /// </summary>
    public interface IGenresResolver
    {
        /// <summary>
        ///     Gets the blockbuster genres.
        /// </summary>
        /// <returns>The blockbuster genres.</returns>
        IEnumerable<Genre> GetBlockbusterGenres();

        /// <summary>
        ///     Gets the minoritary (niche) genres.
        /// </summary>
        /// <returns>The minoritary (niche) genres.</returns>
        IEnumerable<Genre> GetMinoritaryGenres();
    }
}
