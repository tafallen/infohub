using Xunit;
using Moq;
using uk.me.timallen.infohub;
using RestSharp;
using System.Threading.Tasks;
using System.Threading;

namespace infohub.tests
{
    public class SunriseSunsetServiceTests
    {
        [Fact]
        public async Task GetSunriseSunsetTimesAsync_ReturnsFormattedJson()
        {
            // Arrange
            var mockFactory = new Mock<IRestClientFactory>();
            var mockClient = new Mock<IRestClient>();
            var mockResponse = new Mock<IRestResponse>();

            var json = "{ \"results\": { \"sunrise\": \"6:00:00 AM\", \"sunset\": \"6:00:00 PM\" } }";
            mockResponse.Setup(r => r.Content).Returns(json);
            mockClient.Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse.Object);
            mockFactory.Setup(f => f.Create(It.IsAny<string>())).Returns(mockClient.Object);

            var service = new SunriseSunsetService(mockFactory.Object);

            // Act
            var result = await service.GetSunriseSunsetTimesAsync("10", "10");

            // Assert
            Assert.Contains("\"sunrise\":\"6:00:00 AM\"", result);
            Assert.Contains("\"sunset\":\"6:00:00 PM\"", result);
        }
    }
}
