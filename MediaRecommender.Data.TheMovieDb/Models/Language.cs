using System.Text.Json.Serialization;

namespace MediaRecommender.Data.TheMovieDb.Models
{
    public class Language
    {
        #region Properties

        [JsonPropertyName("iso_639_1")]
        public string Iso639Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        #endregion
    }
}
