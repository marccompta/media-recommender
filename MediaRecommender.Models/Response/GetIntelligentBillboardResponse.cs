using System.Collections.Generic;

namespace MediaRecommender.Models.Response
{
    public class GetIntelligentBillboardResponse
    {
        #region Properties

        public IEnumerable<IntelligentBillboard> IntelligentBillboard { get; set; }

        #endregion
    }
}
