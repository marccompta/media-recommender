using System.Text.Json.Serialization;

namespace MediaRecommender.Data.TheMovieDb.Models
{
    public class Genre
    {
        #region Properties

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        #endregion
    }
}
