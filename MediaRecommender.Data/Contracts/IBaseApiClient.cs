using System;
using System.Threading.Tasks;

namespace MediaRecommender.Data.Contracts
{
    /// <summary>
    ///     Interface to personalize the ApiClient to each client.
    /// </summary>
    public interface IBaseApiClient
    {
        #region Public Methods

        /// <summary>
        ///     Gets the string asynchronously.
        /// </summary>
        /// <param name="apiUrl">The API URL.</param>
        /// <returns>The string response.</returns>
        Task<string> GetStringAsync(Uri apiUrl);

        #endregion
    }
}
