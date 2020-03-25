namespace MediaRecommender.Models
{
    public class TVShowRecommendation : Recommendation
    {
        #region Properties

        public string NumberOfSeasons { get; set; }

        public string NumberOfEpisodes { get; set; }

        public string IsConcluded { get; set; }

        #endregion
    }
}
