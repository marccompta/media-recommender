using MediaRecommender.Models;
using System;
using System.Linq;

namespace MediaRecommender.Application.Impl.Mappers
{
    /// <summary>
    ///     A mapper between Domain and Application layer.
    /// </summary>
    /// <seealso cref="IMapper" />
    public class Mapper : IMapper
    {
        public Recommendation DomainToApplicationModel(Entities.Recommendation recommendation)
            => new Recommendation
            {
                Title = recommendation.Title,
                Overview = recommendation.Overview,
                Genre = recommendation.Genres.Select(g => Enum.GetName(typeof(Entities.Genre), g)),
                Language = recommendation.Language,
                ReleaseDate = recommendation.ReleaseDate,
                Website = recommendation.Website,
                Keywords = recommendation.Keywords
            };
    }
}
