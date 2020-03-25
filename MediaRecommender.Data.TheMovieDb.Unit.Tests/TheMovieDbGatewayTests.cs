using AutoFixture;
using FluentAssertions;
using MediaRecommender.Data.TheMovieDb.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MediaRecommender.Data.TheMovieDb.Unit.Tests
{
    public class TheMovieDbGatewayTests
    {
        #region The MakeDiscoverMoviesRequestAsync Method

        public class TheMakeDiscoverMoviesRequestAsyncMethod
        {
            private readonly Fixture _fixture = new Fixture();

            [Fact]
            public async void WhenParametersAreProvided_TheUrlIsProperlyPassed()
            {
                // Arrange
                var apiClientMock = new Mock<IApiClient>();

                Uri uriSpy = null;
                apiClientMock.Setup(m => m.GetAsync<DiscoverMoviesResponse>(It.IsAny<Uri>()))
                    .Callback<Uri>(uri =>
                    {
                        uriSpy = uri;
                    })
                    .ReturnsAsync(_fixture.Create<DiscoverMoviesResponse>());

                var from = _fixture.Create<DateTime>();
                var to = _fixture.Create<DateTime>();
                var withGenres = _fixture.Create<IEnumerable<int>>();
                var withKeywords = _fixture.Create<IEnumerable<string>>();
                var page = _fixture.Create<int>();

                var sut = new TheMovieDbGateway(apiClientMock.Object);

                // Act
                await sut.MakeDiscoverMoviesRequestAsync(from, to, withGenres, withKeywords, page);

                // Assert
                uriSpy.ToString().Should().BeEquivalentTo($"https://" + $"api.themoviedb.org/3/discover/movie?include_adult=false&sort_by=popularidy.desc&primary_release_date.gte={from.ToString("yyyy-MM-dd")}" + 
                    $"&primary_release_date.lte={to.ToString("yyyy-MM-dd")}&" + $"with_genres={ string.Join("|", withGenres) }&" + $"with_keywords={string.Join("|", withKeywords)}&" +
                    $"page={page}");
            }
        }

        #endregion

        #region The MakeMovieDetailRequestAsync Method

        public class TheMakeMovieDetailRequestAsyncMethod
        {
            private readonly Fixture _fixture = new Fixture();

            [Fact]
            public async void WhenParametersAreProvided_TheUrlIsProperlyPassed()
            {
                // Arrange
                var apiClientMock = new Mock<IApiClient>();

                Uri uriSpy = null;
                apiClientMock.Setup(m => m.GetAsync<Movie>(It.IsAny<Uri>()))
                    .Callback<Uri>(uri =>
                    {
                        uriSpy = uri;
                    })
                    .ReturnsAsync(_fixture.Create<Movie>());

                var movieId = _fixture.Create<int>();

                var sut = new TheMovieDbGateway(apiClientMock.Object);

                // Act
                await sut.MakeMovieDetailRequestAsync(movieId, new[] { MovieDetailRequestIncludeTypes.Keywords });

                // Assert
                uriSpy.ToString().Should().BeEquivalentTo("https://" + $"api.themoviedb.org/3/movie/{movieId}?append_to_response=keywords");
            }
        }

        #endregion

        #region The MakeMovieDetailRequestAsync Method

        public class TheMakeSearchMovieRequestAsyncMethod
        {
            private readonly Fixture _fixture = new Fixture();

            [Fact]
            public async void WhenParametersAreProvided_TheUrlIsProperlyPassed()
            {
                // Arrange
                var apiClientMock = new Mock<IApiClient>();

                Uri uriSpy = null;
                apiClientMock.Setup(m => m.GetAsync<SearchMovieResponse>(It.IsAny<Uri>()))
                    .Callback<Uri>(uri =>
                    {
                        uriSpy = uri;
                    })
                    .ReturnsAsync(_fixture.Create<SearchMovieResponse>());

                string title = _fixture.Create<string>();
                int releaseYear = _fixture.Create<DateTime>().Year;

                var sut = new TheMovieDbGateway(apiClientMock.Object);

                // Act
                await sut.MakeSearchMovieRequestAsync(title, releaseYear);

                // Assert
                uriSpy.ToString().Should().BeEquivalentTo("https://" + $"api.themoviedb.org/3/search/movie?query={title}&year={releaseYear}");
            }
        }

        #endregion

        #region The MakeMovieDetailRequestAsync Method

        public class TheMakeMovieKeywordsRequestAsyncMethod
        {
            private readonly Fixture _fixture = new Fixture();

            [Fact]
            public async void WhenParametersAreProvided_TheUrlIsProperlyPassed()
            {
                // Arrange
                var apiClientMock = new Mock<IApiClient>();

                Uri uriSpy = null;
                apiClientMock.Setup(m => m.GetAsync<GetMovieKeywordsResponse>(It.IsAny<Uri>()))
                    .Callback<Uri>(uri =>
                    {
                        uriSpy = uri;
                    })
                    .ReturnsAsync(_fixture.Create<GetMovieKeywordsResponse>());

                var movieId = _fixture.Create<int>();

                var sut = new TheMovieDbGateway(apiClientMock.Object);

                // Act
                await sut.MakeMovieKeywordsRequestAsync(movieId);

                // Assert
                uriSpy.ToString().Should().BeEquivalentTo("https://" + $"api.themoviedb.org/3/movie/{movieId}/keywords");
            }
        }

        #endregion
    }
}
