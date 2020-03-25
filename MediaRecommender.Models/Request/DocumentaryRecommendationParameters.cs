using System.Collections.Generic;

namespace MediaRecommender.Models.Request
{
    public class DocumentaryRecommendationParameters
    {
        #region Properties

        public IEnumerable<string> Topics { get; set; }

        #endregion
    }
}
