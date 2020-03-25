using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MediaRecommender.Data.TheMovieDb.Models
{
    public class GetMovieKeywordsResponse
    {
        #region Properties

        [JsonPropertyName("keywords")]
        public IEnumerable<Keyword> Keywords { get; set; }

        #endregion
    }
}
