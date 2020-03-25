using System.Collections.Generic;
using System.Linq;

namespace MediaRecommender.Data.TheMovieDb.Mappers
{
    /// <summary>
    ///     Maps TheMovieDb and Data layer object models.
    /// </summary>
    /// <seealso cref="MediaRecommender.Data.TheMovieDb.Mappers.IMapper" />
    public class Mapper : IMapper
    {
        #region Properties

        private IDictionary<Data.Models.Genre, Models.Genre> _TMDbToDataModelGenresDictionary;
        private IDictionary<int, Data.Models.Genre> _dataModelToTMDbGenresDictionary;

        #endregion

        #region Constructor

        public Mapper()
        {

        }

        #endregion

        #region Interface Implementation

        public Models.Genre Map(Data.Models.Genre genre)
        {
            var dict = GetTMDbToDataModelGenresDictionary();
            var result = dict.ContainsKey(genre) ? dict[genre] : new Models.Genre { Id = 0, Name = "Undefined" };

            return result;
        }

        public Data.Models.Genre Map(int id)
        {
            var dict = GetDataModelToTMDbGenresDictionary();
            var result = dict[id];

            return result;
        }

        public Data.Models.Movie Map(Models.Movie movie)
            => movie == null 
            ? 
                null 
            : 
                new Data.Models.Movie
                {
                    Title = movie.Title,
                    Overview = movie.Overview,
                    Genres = movie.Genres.Select(g => Map(g.Id)),
                    Iso639LanguageCode = movie.OriginalLanguage,
                    ReleaseDate = movie.ReleaseDate,
                    Website = movie.Homepage,
                    Keywords = movie.KeywordsEnvelope.Keywords.Select(k => k.Name)
                };

        public Data.Models.GetMoviesResponse Map(Models.DiscoverMoviesResponse discoverMoviesResponse)
            => discoverMoviesResponse != null 
            ?
                new Data.Models.GetMoviesResponse
                {
                    Page = discoverMoviesResponse.Page,
                    TotalPages = discoverMoviesResponse.TotalPages,
                    TotalResults = discoverMoviesResponse.TotalResults,
                    Results = discoverMoviesResponse.Results.Select(r => Map(r))
                }
            : 
                new Data.Models.GetMoviesResponse
                {
                    Page = 0,
                    TotalPages = 0,
                    TotalResults = 0,
                    Results = Enumerable.Empty<Data.Models.Movie>()
                };

        #endregion

        #region Private Methods

        private IDictionary<Data.Models.Genre, Models.Genre> GetTMDbToDataModelGenresDictionary()
            => _TMDbToDataModelGenresDictionary ?? CreateTMDbToDataModelGenresDictionary();

        private IDictionary<int, Data.Models.Genre> GetDataModelToTMDbGenresDictionary()
            => _dataModelToTMDbGenresDictionary ?? CreateDataModelToTMDbGenresDictionary();

        private IDictionary<Data.Models.Genre, Models.Genre> CreateTMDbToDataModelGenresDictionary()
            => _TMDbToDataModelGenresDictionary = new Dictionary<Data.Models.Genre, Models.Genre>()
                {
                    { Data.Models.Genre.Undefined, null },
                    { Data.Models.Genre.Action, new Models.Genre { Id = 28, Name = "Action" } },
                    { Data.Models.Genre.Adventure, new Models.Genre{ Id = 12, Name = "Adventure" } },
                    { Data.Models.Genre.ActionAndAdventure, new Models.Genre{ Id = 10759, Name = "Action & Adventure" } },
                    { Data.Models.Genre.Animation, new Models.Genre{ Id = 16, Name = "Animation" } },
                    { Data.Models.Genre.Comedy, new Models.Genre{ Id = 35, Name = "Comedy" } },
                    { Data.Models.Genre.Crime, new Models.Genre{ Id = 80, Name = "Crime" } },
                    { Data.Models.Genre.Drama, new Models.Genre{ Id = 18, Name = "Drama" } },
                    { Data.Models.Genre.Documentary, new Models.Genre{ Id = 99, Name = "Documentary" } },
                    { Data.Models.Genre.Family, new Models.Genre{ Id = 10751, Name = "Family" } },
                    { Data.Models.Genre.Fantasy, new Models.Genre{ Id = 14, Name = "Fantasy" } },
                    { Data.Models.Genre.History, new Models.Genre{ Id = 36, Name = "History" } },
                    { Data.Models.Genre.Horror, new Models.Genre{ Id = 27, Name = "Horror" } },
                    { Data.Models.Genre.Kids, new Models.Genre{ Id = 10762, Name = "Kids" } },
                    { Data.Models.Genre.Music, new Models.Genre{ Id = 10402, Name = "Music" } },
                    { Data.Models.Genre.Mystery, new Models.Genre{ Id = 9648, Name = "Mystery" } },
                    { Data.Models.Genre.News, new Models.Genre{ Id = 10763, Name = "News" } },
                    { Data.Models.Genre.Reality, new Models.Genre{ Id = 10764, Name = "Reality" } },
                    { Data.Models.Genre.Romance, new Models.Genre{ Id = 10749, Name = "Romance" } },
                    { Data.Models.Genre.ScienceFiction, new Models.Genre{ Id = 878, Name = "Science Fiction" } },
                    { Data.Models.Genre.SciFiAndFantasy, new Models.Genre{ Id = 10765, Name = "Sci-Fi & Fantasy" } },
                    { Data.Models.Genre.Soap, new Models.Genre{ Id = 10766, Name = "Soap" } },
                    { Data.Models.Genre.Talk, new Models.Genre{ Id = 10767, Name = "Talk" } },
                    { Data.Models.Genre.Thriller, new Models.Genre{ Id = 53, Name = "Thriller" } },
                    { Data.Models.Genre.TvMovie, new Models.Genre{ Id = 10770, Name = "TV Movie" } },
                    { Data.Models.Genre.War, new Models.Genre{ Id = 10752, Name = "War" } },
                    { Data.Models.Genre.WarAndPolitics, new Models.Genre{ Id = 10768, Name = "War & Politics" } },
                    { Data.Models.Genre.Western, new Models.Genre{ Id = 37, Name = "Western" } }
                };

        private IDictionary<int, Data.Models.Genre> CreateDataModelToTMDbGenresDictionary()
            => _dataModelToTMDbGenresDictionary = new Dictionary<int, Data.Models.Genre>()
                {
                    { 28, Data.Models.Genre.Action },
                    { 12, Data.Models.Genre.Adventure },
                    { 10759, Data.Models.Genre.ActionAndAdventure },
                    { 16, Data.Models.Genre.Animation },
                    { 35, Data.Models.Genre.Comedy },
                    { 80, Data.Models.Genre.Crime },
                    { 18, Data.Models.Genre.Drama },
                    { 99, Data.Models.Genre.Documentary },
                    { 10751, Data.Models.Genre.Family },
                    { 14, Data.Models.Genre.Fantasy },
                    { 36, Data.Models.Genre.History },
                    { 27, Data.Models.Genre.Horror },
                    { 10762, Data.Models.Genre.Kids },
                    { 10402, Data.Models.Genre.Music },
                    { 9648, Data.Models.Genre.Mystery },
                    { 10763, Data.Models.Genre.News },
                    { 10764, Data.Models.Genre.Reality },
                    { 10749, Data.Models.Genre.Romance },
                    { 878, Data.Models.Genre.ScienceFiction },
                    { 10765, Data.Models.Genre.SciFiAndFantasy },
                    { 10766, Data.Models.Genre.Soap },
                    { 10767, Data.Models.Genre.Talk },
                    { 53, Data.Models.Genre.Thriller },
                    { 10770, Data.Models.Genre.TvMovie },
                    { 10752, Data.Models.Genre.War },
                    { 10768, Data.Models.Genre.WarAndPolitics },
                    { 37, Data.Models.Genre.Western }
                };

        #endregion
    }
}
