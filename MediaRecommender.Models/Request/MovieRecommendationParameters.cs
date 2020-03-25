using System;
using System.Collections.Generic;

namespace MediaRecommender.Models.Request
{
    public class MovieRecommendationParameters
    {
        #region Properties

        public DateTime? From { get;  set; }

        public DateTime? To { get; set; }

        public IEnumerable<string> Keywords { get; set; }

        public IEnumerable<string> Genres { get; set; }

        #endregion
    }
}
