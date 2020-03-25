namespace MediaRecommender.Data.TheMovieDb.Mappers
{
    /// <summary>
    ///     Maps TheMovieDb and Data layer object models.
    /// </summary>
    public interface IMapper
    {
        #region Public Methods

        Models.Genre Map(Data.Models.Genre genre);

        Data.Models.GetMoviesResponse Map(Models.DiscoverMoviesResponse discoverMoviesResponse);

        Data.Models.Movie Map(Models.Movie movie);

        #endregion
    }
}
