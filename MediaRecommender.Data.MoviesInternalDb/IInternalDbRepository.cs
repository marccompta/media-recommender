using MediaRecommender.Data.MoviesInternalDb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaRecommender.Data.MoviesInternalDb
{
    /// <summary>
    ///     Interface for the internal database repository.
    /// </summary>
    public interface IInternalDbRepository
    {
        #region Public Methods

        /// <summary>
        ///     Gets the top 5 successful movies in a city by city name.
        /// </summary>
        /// <param name="cityName">Name of the city.</param>
        /// <returns>The top 5 successful movies in the provided city.</returns>
        Task<IEnumerable<SuccessfulMovie>> GetSuccessfulMoviesByCityName(string cityName);

        #endregion
    }
}
