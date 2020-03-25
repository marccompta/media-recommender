using System;
using System.Threading.Tasks;

namespace MediaRecommender.Data
{
    /// <summary>
    ///     Client to request external APIs.
    /// </summary>
    public interface IApiClient
    {
        #region Public Method

        /// <summary>
        ///     Gets the specified resource given its URI.
        /// </summary>
        /// <typeparam name="T">The type of expected resource.</typeparam>
        /// <param name="uri">The URI of the resource.</param>
        /// <returns>Object of type T representing the returned resource.</returns>
        Task<T> GetAsync<T>(Uri uri) where T : class;

        #endregion
    }
}
