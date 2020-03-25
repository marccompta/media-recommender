using AutoFixture;
using FluentAssertions;
using MediaRecommender.Data.Models;
using MediaRecommender.Data.TheMovieDb.Mappers;
using MediaRecommender.Data.TheMovieDb.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MediaRecommender.Data.TheMovieDb.Unit.Tests
{
    public class TheMovieDbAdapterTests
    {
        #region The GetMoviesAsync Method

        public class TheGetMoviesAsyncMethod
        {
            private static readonly Fixture _fixture = new Fixture();

            [Fact]
            public async void WhenMoviesAreFound_ThenAreProperlyEnriched()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<TheMovieDbAdapter>>();

                var discoverMoviesResponse = _fixture.Create<DiscoverMoviesResponse>();
                var movieDetailResponse = _fixture.Create<Models.Movie>();
                var theMovieDbGatewayMock = new Mock<ITheMovieDbGateway>();
                theMovieDbGatewayMock.Setup(g => g.MakeDiscoverMoviesRequestAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<int>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<int>()))
                    .ReturnsAsync(discoverMoviesResponse);
                theMovieDbGatewayMock.Setup(g => g.MakeMovieDetailRequestAsync(It.IsAny<int>(), It.IsAny<IEnumerable<MovieDetailRequestIncludeTypes>>()))
                    .ReturnsAsync(movieDetailResponse);

                DiscoverMoviesResponse spy = null;
                var mapperMock = new Mock<IMapper>();
                mapperMock.Setup(m => m.Map(It.IsAny<DiscoverMoviesResponse>()))
                    .Callback<DiscoverMoviesResponse>(r =>
                    {
                        spy = r;
                    })
                    .Returns(_fixture.Create<GetMoviesResponse>());

                var sut = new TheMovieDbAdapter(loggerMock.Object, theMovieDbGatewayMock.Object, mapperMock.Object);

                // Act.
                await sut.GetMoviesAsync(_fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<IEnumerable<Data.Models.Genre>>(), _fixture.Create<IEnumerable<string>>(), _fixture.Create<int>());

                // Assert
                spy.Results.ToList().Select(r => r.Homepage).Should().BeEquivalentTo(new string[spy.Results.Count()].Select(i => movieDetailResponse.Homepage));
                spy.Results.ToList().Select(r => r.KeywordsEnvelope).Should().BeEquivalentTo(new string[spy.Results.Count()].Select(i => movieDetailResponse.KeywordsEnvelope));
                spy.Results.ToList().Select(r => string.Join(",", r.Genres)).Should().BeEquivalentTo(new string[spy.Results.Count()].Select(i => string.Join(",", movieDetailResponse.Genres)));
            }

            [Fact]
            public async void WhenMakeDiscoverMoviesRequestAsyncThrowsException_ThenProcessContinues()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<TheMovieDbAdapter>>();

                var theMovieDbGatewayMock = new Mock<ITheMovieDbGateway>();
                theMovieDbGatewayMock.Setup(g => g.MakeDiscoverMoviesRequestAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<int>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<int>()))
                    .ThrowsAsync(new Exception());

                DiscoverMoviesResponse spy = null;
                var mapperMock = new Mock<IMapper>();
                mapperMock.Setup(m => m.Map(It.IsAny<DiscoverMoviesResponse>()))
                    .Callback<DiscoverMoviesResponse>(r =>
                    {
                        spy = r;
                    })
                    .Returns(_fixture.Create<GetMoviesResponse>());

                var sut = new TheMovieDbAdapter(loggerMock.Object, theMovieDbGatewayMock.Object, mapperMock.Object);

                // Act.
                await sut.GetMoviesAsync(_fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<IEnumerable<Data.Models.Genre>>(), _fixture.Create<IEnumerable<string>>(), _fixture.Create<int>());

                // Assert
                spy.Should().BeNull();
            }

            [Fact]
            public async void WhenMakeMovieDetailRequestAsyncThrowsException_ThenProcessContinues()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<TheMovieDbAdapter>>();

                var discoverMoviesResponse = _fixture.Build<DiscoverMoviesResponse>()
                                                .With(v => v.Results, _fixture
                                                .CreateMany<Models.Movie>(1))
                                                .Create();

                var theMovieDbGatewayMock = new Mock<ITheMovieDbGateway>();
                theMovieDbGatewayMock.Setup(g => g.MakeDiscoverMoviesRequestAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<int>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<int>()))
                    .ReturnsAsync(discoverMoviesResponse);
                theMovieDbGatewayMock.Setup(g => g.MakeMovieDetailRequestAsync(It.IsAny<int>(), It.IsAny<IEnumerable<MovieDetailRequestIncludeTypes>>()))
                    .ThrowsAsync(new Exception());

                DiscoverMoviesResponse spy = null;
                var mapperMock = new Mock<IMapper>();
                mapperMock.Setup(m => m.Map(It.IsAny<DiscoverMoviesResponse>()))
                    .Callback<DiscoverMoviesResponse>(r =>
                    {
                        spy = r;
                    })
                    .Returns(_fixture.Create<GetMoviesResponse>());

                var sut = new TheMovieDbAdapter(loggerMock.Object, theMovieDbGatewayMock.Object, mapperMock.Object);

                // Act.
                await sut.GetMoviesAsync(_fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<IEnumerable<Data.Models.Genre>>(), _fixture.Create<IEnumerable<string>>(), _fixture.Create<int>());
                var result = spy.Results.ToList()[0];

                // Assert
                result.KeywordsEnvelope.Should().BeEquivalentTo(new GetMovieKeywordsResponse { Keywords = Enumerable.Empty<Keyword>() });
                result.Homepage.Should().Be(string.Empty);
                result.Genres.Should().BeEquivalentTo(Enumerable.Empty<Models.Genre>());
            }
        }

        #endregion

        #region TheGetMovieExternalProviderKeywordIdsAsync Method

        public class TheGetMovieExternalProviderKeywordIdsAsyncMethod
        {
            private static readonly Fixture _fixture = new Fixture();

            [Fact]
            public async void WhenMultipleMoviesAreRetrieved_ThenTheKeywordIdsOfTheFirstOneAreRequested()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<TheMovieDbAdapter>>();

                var theMovieDbGatewayMock = new Mock<ITheMovieDbGateway>();
                var moviesReturned = _fixture.CreateMany<Models.Movie>(3);
                var searchMoviesResponse = _fixture.Build<SearchMovieResponse>()
                                                .With(a => a.Results, moviesReturned)
                                                .Create();
                theMovieDbGatewayMock.Setup(g => g.MakeSearchMovieRequestAsync(It.IsAny<string>(), It.IsAny<int>()))
                    .ReturnsAsync(searchMoviesResponse);

                int requestedMovieId = 0;
                theMovieDbGatewayMock.Setup(g => g.MakeMovieKeywordsRequestAsync(It.IsAny<int>()))
                    .Callback<int>(id =>
                    {
                        requestedMovieId = id;
                    })
                    .ReturnsAsync(_fixture.Create<GetMovieKeywordsResponse>());

                var mapperMock = new Mock<IMapper>();

                var sut = new TheMovieDbAdapter(loggerMock.Object, theMovieDbGatewayMock.Object, mapperMock.Object);

                // Act
                await sut.GetMovieExternalProviderKeywordIdsAsync(_fixture.Create<string>(), _fixture.Create<int>());

                // Assert
                requestedMovieId.Should().Be(moviesReturned.ToList()[0].Id);
            }

            [Fact]
            public async void WhenNoResultsAreFound_ThenReturnEmptyList()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<TheMovieDbAdapter>>();

                var theMovieDbGatewayMock = new Mock<ITheMovieDbGateway>();

                var searchMoviesEmptyResponse = _fixture.Build<SearchMovieResponse>()
                                                .With(a => a.Results, Enumerable.Empty<Models.Movie>())
                                                .With(a => a.TotalResults, 0)
                                                .Create();
                theMovieDbGatewayMock.Setup(g => g.MakeSearchMovieRequestAsync(It.IsAny<string>(), It.IsAny<int>()))
                    .ReturnsAsync(searchMoviesEmptyResponse);

                var mapperMock = new Mock<IMapper>();

                var sut = new TheMovieDbAdapter(loggerMock.Object, theMovieDbGatewayMock.Object, mapperMock.Object);

                // Act
                var result = await sut.GetMovieExternalProviderKeywordIdsAsync(_fixture.Create<string>(), _fixture.Create<int>());

                // Assert
                theMovieDbGatewayMock.Verify(m => m.MakeMovieKeywordsRequestAsync(It.IsAny<int>()), Times.Never);
                result.Count().Should().Be(0);
            }

            [Fact]
            public async void WhenMakeSearchMovieRequestAsyncThrowsException_ThenReturnEmptyList()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<TheMovieDbAdapter>>();

                var theMovieDbGatewayMock = new Mock<ITheMovieDbGateway>();
                theMovieDbGatewayMock.Setup(g => g.MakeSearchMovieRequestAsync(It.IsAny<string>(), It.IsAny<int>()))
                    .ThrowsAsync(new Exception());

                var mapperMock = new Mock<IMapper>();

                var sut = new TheMovieDbAdapter(loggerMock.Object, theMovieDbGatewayMock.Object, mapperMock.Object);

                // Act
                var result = await sut.GetMovieExternalProviderKeywordIdsAsync(_fixture.Create<string>(), _fixture.Create<int>());

                // Assert
                theMovieDbGatewayMock.Verify(m => m.MakeMovieKeywordsRequestAsync(It.IsAny<int>()), Times.Never);
                result.Count().Should().Be(0);
            }

            [Fact]
            public async void WhenMakeMovieKeywordsRequestAsyncThrowsException_ThenReturnEmptyList()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<TheMovieDbAdapter>>();

                var theMovieDbGatewayMock = new Mock<ITheMovieDbGateway>();
                var searchMoviesResponse = _fixture.Create<SearchMovieResponse>();
                theMovieDbGatewayMock.Setup(g => g.MakeSearchMovieRequestAsync(It.IsAny<string>(), It.IsAny<int>()))
                    .ReturnsAsync(searchMoviesResponse);

                theMovieDbGatewayMock.Setup(g => g.MakeMovieKeywordsRequestAsync(It.IsAny<int>()))
                    .ThrowsAsync(new Exception());

                var mapperMock = new Mock<IMapper>();

                var sut = new TheMovieDbAdapter(loggerMock.Object, theMovieDbGatewayMock.Object, mapperMock.Object);

                // Act
                var result = await sut.GetMovieExternalProviderKeywordIdsAsync(_fixture.Create<string>(), _fixture.Create<int>());

                // Assert
                theMovieDbGatewayMock.Verify(m => m.MakeSearchMovieRequestAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
                result.Count().Should().Be(0);
            }
        }

        #endregion
    }
}
