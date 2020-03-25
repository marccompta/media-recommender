using AutoFixture;
using FluentAssertions;
using MediaRecommender.Contracts;
using MediaRecommender.Data.MoviesInternalDb;
using MediaRecommender.Data.MoviesInternalDb.Models;
using MediaRecommender.Entities;
using MediaRecommender.Repositories;
using MediaRecommender.Resolvers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MediaRecommender.Unit.Tests
{
    public class IntelligentBillboardResolverTests
    {
        #region The ResolveAsync Method

        public class TheResolveAsyncMethod
        {
            private static readonly Fixture _fixture = new Fixture();

            [Fact]
            public void IfNoCityIsProvided_ThenOk()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<IntelligentBillboardResolver>>();

                var billboardRepositoryMock = new Mock<IBillboardRepository>();
                billboardRepositoryMock.Setup(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<int>()))
                    .ReturnsAsync(_fixture.Create<GetMoviesBillboardRepositoryResponse>());

                var internalDbRepositoryMock = new Mock<IInternalDbRepository>();
                internalDbRepositoryMock.Setup(r => r.GetSuccessfulMoviesByCityName(It.IsAny<string>()))
                    .ReturnsAsync(_fixture.Create<IEnumerable<SuccessfulMovie>>());

                var genresResolverMock = new Mock<IGenresResolver>();
                genresResolverMock.Setup(r => r.GetBlockbusterGenres()).Returns(_fixture.Create<IEnumerable<Genre>>);
                genresResolverMock.Setup(r => r.GetMinoritaryGenres()).Returns(_fixture.Create<IEnumerable<Genre>>);

                DateTime from = _fixture.Create<DateTime>();
                int numberOfWeeks = _fixture.Create<int>();
                int numberOfBigRooms = _fixture.Create<int>();
                int numberOfSmallRooms = _fixture.Create<int>();

                var sut = new IntelligentBillboardResolver(loggerMock.Object, billboardRepositoryMock.Object, internalDbRepositoryMock.Object, genresResolverMock.Object);

                // Act
                Func<Task> func = async () => await sut.ResolveAsync(from, numberOfWeeks, numberOfBigRooms, numberOfSmallRooms, string.Empty);

                // Assert
                func.Should().NotThrow<Exception>();
            }
        }

        #endregion

        #region The GenerateBillboardAsync Method

        public class TheGenerateBillboardAsyncMethod
        {
            private static readonly Fixture _fixture = new Fixture();

            [Fact]
            public async void WhenThereAreRepeatedMoviesInTheRepoResponse_ThenTheseAreFilteredOff()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<IntelligentBillboardResolver>>();
                string movieTitle1 = _fixture.Create<string>();
                string movieTitle2 = _fixture.Create<string>();
                string movieTitle3 = _fixture.Create<string>();
                string movieTitle4 = _fixture.Create<string>();
                string movieTitle5 = _fixture.Create<string>();

                List<Recommendation> returnedRecommendations = new List<Recommendation> 
                {
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle4).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle4).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle5).Create(),
                };

                var billboardRepositoryMock = new Mock<IBillboardRepository>();
                billboardRepositoryMock.Setup(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<int>()))
                    .ReturnsAsync
                    (
                        _fixture.Build<GetMoviesBillboardRepositoryResponse>()
                            .With(r => r.Page, 1)
                            .With(r => r.TotalResults, 200)
                            .With(r => r.TotalPages, 10)
                            .With(r => r.Results, returnedRecommendations)
                            .Create()
                    );

                var moviesAlreadyAssigned = new ConcurrentDictionary<string, byte>();

                var sut = new IntelligentBillboardResolver(loggerMock.Object, billboardRepositoryMock.Object, null, null);

                // Act
                var result = (await sut.GenerateBillboardAsync(_fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<IEnumerable<Genre>>(), _fixture.Create<IEnumerable<string>>(), 5, moviesAlreadyAssigned)).ToList();

                // Assert
                result.Where(r => r.Title == movieTitle1).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle2).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle3).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle4).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle5).Count().Should().Be(1);
            }

            [Fact]
            public async void WhenThereAreNotEnoughMoviesInAPage_ThenTheNextOneIsRequested()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<IntelligentBillboardResolver>>();
                string movieTitle1 = _fixture.Create<string>();
                string movieTitle2 = _fixture.Create<string>();
                string movieTitle3 = _fixture.Create<string>();
                string movieTitle4 = _fixture.Create<string>();
                string movieTitle5 = _fixture.Create<string>();

                #region Creation of three pages of recommendation movies results that are to be retrieved to achieve the goal.

                List<Recommendation> returnedRecommendations1 = new List<Recommendation>
                {
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                };

                List<Recommendation> returnedRecommendations2 = new List<Recommendation>
                {
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle4).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                };

                List<Recommendation> returnedRecommendations3 = new List<Recommendation>
                {
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle4).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle5).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                };

                #endregion

                var billboardRepositoryMock = new Mock<IBillboardRepository>();
                billboardRepositoryMock.Setup(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 1))
                    .ReturnsAsync
                    (
                        _fixture.Build<GetMoviesBillboardRepositoryResponse>()
                            .With(r => r.Page, 1)
                            .With(r => r.TotalResults, 200)
                            .With(r => r.TotalPages, 10)
                            .With(r => r.Results, returnedRecommendations1)
                            .Create()
                    );
                billboardRepositoryMock.Setup(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 2))
                    .ReturnsAsync
                    (
                        _fixture.Build<GetMoviesBillboardRepositoryResponse>()
                            .With(r => r.Page, 2)
                            .With(r => r.TotalResults, 200)
                            .With(r => r.TotalPages, 10)
                            .With(r => r.Results, returnedRecommendations2)
                            .Create()
                    );
                billboardRepositoryMock.Setup(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 3))
                    .ReturnsAsync
                    (
                        _fixture.Build<GetMoviesBillboardRepositoryResponse>()
                            .With(r => r.Page, 3)
                            .With(r => r.TotalResults, 200)
                            .With(r => r.TotalPages, 10)
                            .With(r => r.Results, returnedRecommendations3)
                            .Create()
                    );

                var moviesAlreadyAssigned = new ConcurrentDictionary<string, byte>();

                var sut = new IntelligentBillboardResolver(loggerMock.Object, billboardRepositoryMock.Object, null, null);

                // Act
                var result = (await sut.GenerateBillboardAsync(_fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<IEnumerable<Genre>>(), _fixture.Create<IEnumerable<string>>(), 5, moviesAlreadyAssigned)).ToList();

                // Assert
                result.Where(r => r.Title == movieTitle1).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle2).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle3).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle4).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle5).Count().Should().Be(1);
                billboardRepositoryMock.Verify(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 1), Times.Once);
                billboardRepositoryMock.Verify(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 2), Times.Once);
                billboardRepositoryMock.Verify(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 3), Times.Once);
            }

            [Fact]
            public async void WhenThereAreNotEnoughMoviesInTheFulLSetOfPages_ThenTheProcessEndsAndCurrentResultsetIsReturned()
            {
                // Arrange
                var loggerMock = new Mock<ILogger<IntelligentBillboardResolver>>();
                string movieTitle1 = _fixture.Create<string>();
                string movieTitle2 = _fixture.Create<string>();
                string movieTitle3 = _fixture.Create<string>();
                string movieTitle4 = _fixture.Create<string>();
                string movieTitle5 = _fixture.Create<string>();

                #region Creation of three pages of recommendation movies results that are to be passed throught to achieve the goal.

                List<Recommendation> returnedRecommendations1 = new List<Recommendation>
                {
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                };

                List<Recommendation> returnedRecommendations2 = new List<Recommendation>
                {
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle4).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                };

                List<Recommendation> returnedRecommendations3 = new List<Recommendation>
                {
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle4).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle5).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle2).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle3).Create(),
                    _fixture.Build<Recommendation>().With(r => r.Title, movieTitle1).Create(),
                };

                #endregion

                var billboardRepositoryMock = new Mock<IBillboardRepository>();
                billboardRepositoryMock.Setup(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 1))
                    .ReturnsAsync
                    (
                        _fixture.Build<GetMoviesBillboardRepositoryResponse>()
                            .With(r => r.Page, 1)
                            .With(r => r.TotalResults, 60)
                            .With(r => r.TotalPages, 3)
                            .With(r => r.Results, returnedRecommendations1)
                            .Create()
                    );
                billboardRepositoryMock.Setup(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 2))
                    .ReturnsAsync
                    (
                        _fixture.Build<GetMoviesBillboardRepositoryResponse>()
                            .With(r => r.Page, 2)
                            .With(r => r.TotalResults, 60)
                            .With(r => r.TotalPages, 3)
                            .With(r => r.Results, returnedRecommendations2)
                            .Create()
                    );
                billboardRepositoryMock.Setup(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 3))
                    .ReturnsAsync
                    (
                        _fixture.Build<GetMoviesBillboardRepositoryResponse>()
                            .With(r => r.Page, 3)
                            .With(r => r.TotalResults, 60)
                            .With(r => r.TotalPages, 3)
                            .With(r => r.Results, returnedRecommendations3)
                            .Create()
                    );

                var moviesAlreadyAssigned = new ConcurrentDictionary<string, byte>();

                var sut = new IntelligentBillboardResolver(loggerMock.Object, billboardRepositoryMock.Object, null, null);

                // Act
                var result = (await sut.GenerateBillboardAsync(_fixture.Create<DateTime>(), _fixture.Create<DateTime>(), _fixture.Create<IEnumerable<Genre>>(), _fixture.Create<IEnumerable<string>>(), 6, moviesAlreadyAssigned)).ToList();

                // Assert
                result.Where(r => r.Title == movieTitle1).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle2).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle3).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle4).Count().Should().Be(1);
                result.Where(r => r.Title == movieTitle5).Count().Should().Be(1);
                billboardRepositoryMock.Verify(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 1), Times.Once);
                billboardRepositoryMock.Verify(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 2), Times.Once);
                billboardRepositoryMock.Verify(r => r.GetMoviesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<Genre>>(), It.IsAny<IEnumerable<string>>(), 3), Times.Once);
                result.Count().Should().Be(5);
            }
        }

        #endregion
    }
}
