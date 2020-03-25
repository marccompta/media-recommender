using MediaRecommender.Contracts;
using MediaRecommender.Entities;
using System.Collections.Generic;

namespace MediaRecommender.Resolvers
{
    /// <summary>
    ///     Resolves the genres that correspond to each category.
    /// </summary>
    /// <seealso cref="IGenresResolver" />
    public class GenresResolver : IGenresResolver
    {
        /// <summary>
        ///     Gets the blockbuster genres.
        /// </summary>
        /// <returns>The blockbuster genres.</returns>
        public IEnumerable<Genre> GetBlockbusterGenres()
            => new List<Genre>
            {
                Genre.Action,
                Genre.Adventure,
                Genre.ActionAndAdventure,
                Genre.Comedy,
                Genre.Drama,
                Genre.ScienceFiction,
                Genre.SciFiAndFantasy,
                Genre.Thriller
            };

        /// <summary>
        ///     Gets the minoritary (niche) genres.
        /// </summary>
        /// <returns>The minoritary (niche) genres.</returns>
        public IEnumerable<Genre> GetMinoritaryGenres()
            => new List<Genre>
            {
                Genre.Animation,
                Genre.Crime,
                Genre.Drama,
                Genre.Family,
                Genre.Fantasy,
                Genre.History,
                Genre.Horror,
                Genre.Kids,
                Genre.Music,
                Genre.Mystery,
                Genre.News,
                Genre.Reality,
                Genre.Romance,
                Genre.ScienceFiction,
                Genre.Soap,
                Genre.Talk,
                Genre.TvMovie,
                Genre.War,
                Genre.WarAndPolitics,
                Genre.Western
            };
    }
}
