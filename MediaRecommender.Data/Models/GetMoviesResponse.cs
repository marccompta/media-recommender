using System.Collections.Generic;

namespace MediaRecommender.Data.Models
{
    public class GetMoviesResponse
    {
        #region Properties

        public int Page { get; set; }

        public int TotalResults { get; set; }

        public int TotalPages { get; set; }

        public IEnumerable<Movie> Results { get; set; }

        #endregion
    }
}
