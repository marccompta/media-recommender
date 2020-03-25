using MediaRecommender.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace MediaRecommender.Data
{
    /// <summary>
    ///     Client to request from external APIs.
    /// </summary>
    public class ApiClient : IApiClient
    {
        #region Fields

        private readonly ILogger<ApiClient> _logger;
        private readonly IBaseApiClient _baseApiClient;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="baseApiClient">The base API client.</param>
        public ApiClient(ILogger<ApiClient> logger, IBaseApiClient baseApiClient)
        {
            _logger = logger;
            _baseApiClient = baseApiClient;
        }

        /// <summary>
        ///     Gets the specified resource given its URI.
        /// </summary>
        /// <typeparam name="T">The type of expected resource.</typeparam>
        /// <param name="uri">The URI of the resource.</param>
        /// <returns>
        ///     Object of type T representing the returned resource.
        /// </returns>
        public async Task<T> GetAsync<T>(Uri uri) where T : class
        {
            T response = null;

            try
            {
                string content = await _baseApiClient.GetStringAsync(uri);
                response = JsonSerializer.Deserialize<T>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error @{nameof(ApiClient)} for [{uri}].");
            }

            return response;
        }

        #endregion
    }
}
