using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaRecommender.Data.MoviesInternalDb.Models
{
    public class SuccessfulMovie
    {
        #region Properties

        [Column("MOVIE_TITLE")]
        public string Title { get; set; }

        [Column("SEATS_SOLD")]
        public int SeatsSold { get; set; }

        [Column("RELEASE_DATE")]
        public DateTime ReleaseDate { get; set; }

        [Column("GENRES")]
        public string Genres { get; set; }

        #endregion
    }
}
