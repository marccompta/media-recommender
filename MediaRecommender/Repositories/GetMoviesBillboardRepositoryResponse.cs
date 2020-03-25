using MediaRecommender.Entities;
using System.Collections.Generic;

namespace MediaRecommender.Repositories
{
    public class GetMoviesBillboardRepositoryResponse
    {
        #region Properties

        public int Page { get; set; }

        public int TotalResults { get; set; }

        public int TotalPages { get; set; }

        public IEnumerable<Recommendation> Results { get; set; }

        #endregion
    }
}
