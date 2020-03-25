using MediaRecommender.Data.Models;
using MediaRecommender.Entities;

namespace MediaRecommender.Contracts
{
    /// <summary>
    ///     Maps between Data and Domain layer objects.
    /// </summary>
    public interface IMapper
    {
        #region Public Methods

        Recommendation ToRecommendation(Movie movie);

        Data.Models.Genre Map(Entities.Genre genre);

        Entities.Genre Map(Data.Models.Genre genre);

        Repositories.GetMoviesBillboardRepositoryResponse Map(GetMoviesResponse model);

        #endregion
    }
}
