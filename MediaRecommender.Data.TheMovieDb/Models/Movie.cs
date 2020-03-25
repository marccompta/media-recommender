using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace MediaRecommender.Data.TheMovieDb.Models
{
    public class Movie
    {
        #region Properties

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("genres")]
        public IEnumerable<Genre> Genres { get; set; }

        [JsonPropertyName("homepage")]
        public string Homepage { get; set; }

        /// <summary>
        /// ISO 3166-1 code.
        /// </summary>
        [JsonPropertyName("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonPropertyName("original_title")]
        public string OriginalTitle { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("release_date")]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("keywords")]
        public GetMovieKeywordsResponse KeywordsEnvelope { get; set; }

        #endregion
    }
}
