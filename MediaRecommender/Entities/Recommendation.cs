using System;
using System.Collections.Generic;

namespace MediaRecommender.Entities
{
    public class Recommendation
    {
        #region Properties

        public string Title { get; set; }

        public string Overview { get; set; }

        public IEnumerable<Genre> Genres { get; set; }

        public string Language { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string Website { get; set; }

        public IEnumerable<string> Keywords { get; set; }

        #endregion
    }
}
