using System.Collections.Generic;

namespace MediaRecommender.Entities
{
    public class IntelligentBillboardRecommendation
    {
        #region Properties

        public int WeekNumber { get; set; }

        public IEnumerable<Recommendation> BigRooms { get; set; }

        public IEnumerable<Recommendation> SmallRooms { get; set; }

        #endregion
    }
}
