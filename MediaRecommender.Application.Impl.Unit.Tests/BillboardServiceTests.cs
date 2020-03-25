using AutoFixture;
using FluentAssertions;
using MediaRecommender.Application.Impl.Mappers;
using MediaRecommender.Entities;
using MediaRecommender.Resolvers;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MediaRecommender.Application.Impl.Unit.Tests
{
    public class BillboardServiceTests
    {
        #region The GetIntelligentBillboardAsync Method

        public class TheGetIntelligentBillboardAsyncMethod
        {
            private static readonly Fixture _fixture = new Fixture();

            [Fact]
            public void WhenFromArgumentIsNull_ThenArgumentNullExceptionIsThrown()
            {
                // Arrange
                var sut = new BillboardService(null, null);

                // Act
                Func<Task> func = async () => await sut.GetIntelligentBillboardAsync(null, _fixture.Create<int>(), _fixture.Create<int>(), _fixture.Create<int>(), _fixture.Create<string>());

                // Assert
                func.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void WhenNumberOfWeeksArgumentIsNull_ThenArgumentNullExceptionIsThrown()
            {
                // Arrange
                var sut = new BillboardService(null, null);

                // Act
                Func<Task> func = async () => await sut.GetIntelligentBillboardAsync(_fixture.Create<DateTime>(), null, _fixture.Create<int>(), _fixture.Create<int>(), _fixture.Create<string>());

                // Assert
                func.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void WhenNumberOfBigRoomsArgumentIsNull_ThenArgumentNullExceptionIsThrown()
            {
                // Arrange
                var sut = new BillboardService(null, null);

                // Act
                Func<Task> func = async () => await sut.GetIntelligentBillboardAsync(_fixture.Create<DateTime>(), _fixture.Create<int>(), null, _fixture.Create<int>(), _fixture.Create<string>());

                // Assert
                func.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void WhenNumberOfSmallRoomsArgumentIsNull_ThenArgumentNullExceptionIsThrown()
            {
                // Arrange
                var sut = new BillboardService(null, null);

                // Act
                Func<Task> func = async () => await sut.GetIntelligentBillboardAsync(_fixture.Create<DateTime>(), _fixture.Create<int>(), _fixture.Create<int>(), null, _fixture.Create<string>());

                // Assert
                func.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void WhenCityArgumentIsNull_ThenOk()
            {
                // Arrange
                var intelligentBillboardResolverMock = new Mock<IIntelligentBillboardResolver>();
                intelligentBillboardResolverMock.Setup(r => r.ResolveAsync(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                    .ReturnsAsync(_fixture.Create<IEnumerable<IntelligentBillboardRecommendation>>());

                var mapperMock = new Mock<IMapper>();
                mapperMock.Setup(r => r.DomainToApplicationModel(It.IsAny<Recommendation>()))
                    .Returns(_fixture.Create<Models.Recommendation>());

                var sut = new BillboardService(intelligentBillboardResolverMock.Object, mapperMock.Object);

                // Act
                Func<Task> func = async () => await sut.GetIntelligentBillboardAsync(_fixture.Create<DateTime>(), _fixture.Create<int>(), _fixture.Create<int>(), _fixture.Create<int>(), null);

                // Assert
                func.Should().NotThrow<Exception>();
            }

            [Fact]
            public async void WhenArgumentsAreValid_ThenResolverIsCalledOnceWithProperParameters()
            {
                // Arrange
                var intelligentBillboardResolverMock = new Mock<IIntelligentBillboardResolver>();
                intelligentBillboardResolverMock.Setup(r => r.ResolveAsync(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                    .ReturnsAsync(_fixture.Create<IEnumerable<IntelligentBillboardRecommendation>>());

                var mapperMock = new Mock<IMapper>();
                mapperMock.Setup(r => r.DomainToApplicationModel(It.IsAny<Recommendation>()))
                    .Returns(_fixture.Create<Models.Recommendation>());

                var from = _fixture.Create<DateTime>();
                var weeks = _fixture.Create<int>();
                var numBigRooms = _fixture.Create<int>();
                var numSmallRooms = _fixture.Create<int>();
                var city = _fixture.Create<string>();

                var sut = new BillboardService(intelligentBillboardResolverMock.Object, mapperMock.Object);

                // Act
                await sut.GetIntelligentBillboardAsync(from, weeks, numBigRooms, numSmallRooms, city);

                // Assert
                intelligentBillboardResolverMock.Verify(r => r.ResolveAsync(from, weeks, numBigRooms, numSmallRooms, city), Times.Once);
            }

            [Fact]
            public async void WhenResolverReturnsNull_ThenMapperIsNotCalled()
            {
                // Arrange
                var intelligentBillboardResolverMock = new Mock<IIntelligentBillboardResolver>();
                intelligentBillboardResolverMock.Setup(r => r.ResolveAsync(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                    .ReturnsAsync((IEnumerable<IntelligentBillboardRecommendation>) null);

                var mapperMock = new Mock<IMapper>();
                mapperMock.Setup(r => r.DomainToApplicationModel(It.IsAny<Recommendation>()))
                    .Returns(_fixture.Create<Models.Recommendation>());

                var from = _fixture.Create<DateTime>();
                var weeks = _fixture.Create<int>();
                var numBigRooms = _fixture.Create<int>();
                var numSmallRooms = _fixture.Create<int>();
                var city = _fixture.Create<string>();

                var sut = new BillboardService(intelligentBillboardResolverMock.Object, mapperMock.Object);

                // Act
                await sut.GetIntelligentBillboardAsync(from, weeks, numBigRooms, numSmallRooms, city);

                // Assert
                mapperMock.Verify(r => r.DomainToApplicationModel(It.IsAny<Recommendation>()), Times.Never);
            }
        }

        #endregion
    }
}
