using System;

namespace MediaRecommender.Models.Request
{
    public class BillboardParameters
    {
        #region Properties

        public int? NumberOfWeeks { get; set; }

        public DateTime? From { get; set; }

        public int? NumberOfScreens { get; set; }

        #endregion
    }
}
