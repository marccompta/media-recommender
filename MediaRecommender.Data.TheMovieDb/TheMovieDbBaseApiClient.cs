using MediaRecommender.Data.Contracts;
using MediaRecommender.Data.TheMovieDb.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MediaRecommender.Data.TheMovieDb
{
    /// <summary>
    ///     Personalize the ApiClient to TMDb.
    /// </summary>
    /// <seealso cref="IBaseApiClient" />
    public class TheMovieDbBaseApiClient : IBaseApiClient
    {
        #region Fields

        private readonly ILogger<ApiClient> _logger;
        private readonly TheMovieDbOptions _options;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="TheMovieDbBaseApiClient"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="optionsProvider">The options provider.</param>
        public TheMovieDbBaseApiClient(ILogger<ApiClient> logger, IOptions<TheMovieDbOptions> optionsProvider)
        {
            _logger = logger;
            _options = optionsProvider.Value;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        ///     Gets the string asynchronously.
        /// </summary>
        /// <param name="apiUrl">The API URL.</param>
        /// <returns>
        /// The string response.
        /// </returns>
        /// <exception cref="Exception">Calling {apiUrl} responded with status {response.StatusCode}.</exception>
        public async Task<string> GetStringAsync(Uri apiUrl)
        {
            var headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer { _options.Token }" }
            };

            var response = await RequestAsync(HttpMethod.Get, apiUrl, headers);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Error @{nameof(TheMovieDbBaseApiClient)} for [{apiUrl}].");
                throw new Exception($"Calling {apiUrl} responded with status {response.StatusCode}.");
            }

            string content = await response.Content.ReadAsStringAsync();

            return content;
        }

        #endregion

        #region Private Methods

        private async Task<HttpResponseMessage> RequestAsync(HttpMethod verb, Uri uri, Dictionary<string, string> headers)
        {
            using (var client = new HttpClient())
            {
                var message = new HttpRequestMessage(verb, uri);

                foreach(var header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }

                var response = await client.SendAsync(message, CancellationToken.None);

                return response;
            }
        }

        #endregion
    }
}
