using MediaRecommender.Contracts;
using MediaRecommender.Data.Models;
using MediaRecommender.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MediaRecommender.Mappers
{
    /// <summary>
    ///     Maps between Data and Domain layer objects.
    /// </summary>
    /// <seealso cref="IMapper" />
    public class Mapper : IMapper
    {
        #region Fields

        private IDictionary<Entities.Genre, Data.Models.Genre> _domainToDataGenresDictionary;
        private IDictionary<Data.Models.Genre, Entities.Genre> _dataToDomainGenresDictionary;

        #endregion

        #region Constructors

        public Mapper()
        {
            
        }

        #endregion

        #region Interface Implementation

        public Recommendation ToRecommendation(Movie movie)
            => new Recommendation
            {
                Title = movie.Title,
                Overview = movie.Overview,
                Genres = movie.Genres.Select(x => Map(x)),
                Language = movie.Iso639LanguageCode,
                ReleaseDate = movie.ReleaseDate,
                Website = movie.Website,
                Keywords = movie.Keywords
            };


        public Data.Models.Genre Map(Entities.Genre genre)
        {
            var mapper = GetDomainToDataGenresDictionary();
            var result = mapper[genre];

            return result;
        }

        public Entities.Genre Map(Data.Models.Genre genre)
        {
            var mapper = GetDataToDomainGenresDictionary();
            var result = mapper[genre];

            return result;
        }

        public Repositories.GetMoviesBillboardRepositoryResponse Map(GetMoviesResponse model)
            => new Repositories.GetMoviesBillboardRepositoryResponse
            {
                Page = model.Page,
                TotalPages = model.TotalPages,
                TotalResults = model.TotalResults,
                Results = model.Results?.Select(r => ToRecommendation(r))
            };

        #endregion

        #region Private Methods

        private IDictionary<Entities.Genre, Data.Models.Genre> GetDomainToDataGenresDictionary()
            => _domainToDataGenresDictionary ?? CreateDomainToDataGenresDictionary();

        private IDictionary<Data.Models.Genre, Entities.Genre> GetDataToDomainGenresDictionary()
            => _dataToDomainGenresDictionary ?? CreateDataToDomainGenresDictionary();

        private IDictionary<Entities.Genre, Data.Models.Genre> CreateDomainToDataGenresDictionary()
            => _domainToDataGenresDictionary = new Dictionary<Entities.Genre, Data.Models.Genre>
                {
                    { Entities.Genre.Undefined, Data.Models.Genre.Undefined },
                    { Entities.Genre.Action, Data.Models.Genre.Action },
                    { Entities.Genre.Adventure, Data.Models.Genre.Adventure },
                    { Entities.Genre.ActionAndAdventure, Data.Models.Genre.ActionAndAdventure },
                    { Entities.Genre.Animation, Data.Models.Genre.Animation },
                    { Entities.Genre.Comedy, Data.Models.Genre.Comedy },
                    { Entities.Genre.Crime, Data.Models.Genre.Crime },
                    { Entities.Genre.Drama, Data.Models.Genre.Drama },
                    { Entities.Genre.Documentary, Data.Models.Genre.Documentary },
                    { Entities.Genre.Family, Data.Models.Genre.Family },
                    { Entities.Genre.Fantasy, Data.Models.Genre.Fantasy },
                    { Entities.Genre.History, Data.Models.Genre.History },
                    { Entities.Genre.Horror, Data.Models.Genre.Horror },
                    { Entities.Genre.Kids, Data.Models.Genre.Kids },
                    { Entities.Genre.Music, Data.Models.Genre.Music },
                    { Entities.Genre.Mystery, Data.Models.Genre.Mystery },
                    { Entities.Genre.News, Data.Models.Genre.News },
                    { Entities.Genre.Reality, Data.Models.Genre.Reality },
                    { Entities.Genre.Romance, Data.Models.Genre.Romance },
                    { Entities.Genre.ScienceFiction, Data.Models.Genre.ScienceFiction },
                    { Entities.Genre.SciFiAndFantasy, Data.Models.Genre.SciFiAndFantasy },
                    { Entities.Genre.Soap, Data.Models.Genre.Soap },
                    { Entities.Genre.Talk, Data.Models.Genre.Talk },
                    { Entities.Genre.Thriller, Data.Models.Genre.Thriller },
                    { Entities.Genre.TvMovie, Data.Models.Genre.TvMovie },
                    { Entities.Genre.War, Data.Models.Genre.War },
                    { Entities.Genre.WarAndPolitics, Data.Models.Genre.WarAndPolitics },
                    { Entities.Genre.Western, Data.Models.Genre.Western }
                };

        private IDictionary<Data.Models.Genre, Entities.Genre> CreateDataToDomainGenresDictionary()
            => _dataToDomainGenresDictionary = new Dictionary<Data.Models.Genre, Entities.Genre>
                {
                    { Data.Models.Genre.Undefined, Entities.Genre.Undefined },
                    { Data.Models.Genre.Action, Entities.Genre.Action },
                    { Data.Models.Genre.Adventure, Entities.Genre.Adventure },
                    { Data.Models.Genre.ActionAndAdventure, Entities.Genre.ActionAndAdventure },
                    { Data.Models.Genre.Animation, Entities.Genre.Animation },
                    { Data.Models.Genre.Comedy, Entities.Genre.Comedy },
                    { Data.Models.Genre.Crime, Entities.Genre.Crime },
                    { Data.Models.Genre.Drama, Entities.Genre.Drama },
                    { Data.Models.Genre.Documentary, Entities.Genre.Documentary },
                    { Data.Models.Genre.Family, Entities.Genre.Family },
                    { Data.Models.Genre.Fantasy, Entities.Genre.Fantasy },
                    { Data.Models.Genre.History, Entities.Genre.History },
                    { Data.Models.Genre.Horror, Entities.Genre.Horror },
                    { Data.Models.Genre.Kids, Entities.Genre.Kids },
                    { Data.Models.Genre.Music, Entities.Genre.Music },
                    { Data.Models.Genre.Mystery, Entities.Genre.Mystery },
                    { Data.Models.Genre.News, Entities.Genre.News },
                    { Data.Models.Genre.Reality, Entities.Genre.Reality },
                    { Data.Models.Genre.Romance, Entities.Genre.Romance },
                    { Data.Models.Genre.ScienceFiction, Entities.Genre.ScienceFiction },
                    { Data.Models.Genre.SciFiAndFantasy, Entities.Genre.SciFiAndFantasy },
                    { Data.Models.Genre.Soap, Entities.Genre.Soap },
                    { Data.Models.Genre.Talk, Entities.Genre.Talk },
                    { Data.Models.Genre.Thriller, Entities.Genre.Thriller },
                    { Data.Models.Genre.TvMovie, Entities.Genre.TvMovie },
                    { Data.Models.Genre.War, Entities.Genre.War },
                    { Data.Models.Genre.WarAndPolitics, Entities.Genre.WarAndPolitics },
                    { Data.Models.Genre.Western, Entities.Genre.Western }
                };

        #endregion
    }
}
