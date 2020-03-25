using MediaRecommender.Models;

namespace MediaRecommender.Application.Impl.Mappers
{
    /// <summary>
    ///     A mapper between Domain and Application layer.
    /// </summary>
    public interface IMapper
    {
        #region Public Methods

        Recommendation DomainToApplicationModel(Entities.Recommendation recommendation);

        #endregion
    }
}
