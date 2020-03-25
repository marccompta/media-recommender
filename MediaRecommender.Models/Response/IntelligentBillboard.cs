using System.Collections.Generic;

namespace MediaRecommender.Models.Response
{
    public class IntelligentBillboard
    {
        #region Properties

        public int WeekNumber { get; set; }

        public IEnumerable<Recommendation> BigRooms { get; set; }

        public IEnumerable<Recommendation> SmallRooms { get; set; }

        #endregion
    }
}
