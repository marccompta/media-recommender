using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MediaRecommender.Data.TheMovieDb.Models
{
    public class SearchMovieResponse
    {
        #region Properties

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages{ get; set; }

        [JsonPropertyName("results")]
        public IEnumerable<Movie> Results { get; set; }

        #endregion
    }
}
