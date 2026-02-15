using Xunit;
using Moq;
using uk.me.timallen.infohub;
using RestSharp;
using System.Threading.Tasks;
using System.Threading;

namespace infohub.tests
{
    public class OpenWeatherServiceTests
    {
        [Fact]
        public async Task GetForecastAsync_ReturnsFormattedJson()
        {
            // Arrange
            var mockFactory = new Mock<IRestClientFactory>();
            var mockClient = new Mock<IRestClient>();
            var mockResponse = new Mock<IRestResponse>();

            var json = @"{
                'daily': [
                    {
                        'temp': { 'min': 10, 'max': 20 },
                        'weather': [ { 'main': 'Clouds', 'icon': '04d' } ],
                        'wind_speed': 5,
                        'wind_deg': 180,
                        'sunrise': 1600000000,
                        'sunset': 1600040000
                    }
                ]
            }";
            mockResponse.Setup(r => r.Content).Returns(json);
            mockClient.Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse.Object);
            mockFactory.Setup(f => f.Create(It.IsAny<string>())).Returns(mockClient.Object);

            var service = new OpenWeatherService(mockFactory.Object);

            // Act
            var result = await service.GetForecastAsync("50", "0");

            // Assert
            Assert.Contains("\"min\": \"10\"", result);
            Assert.Contains("\"max\": \"20\"", result);
            Assert.Contains("\"summary\":\"Clouds\"", result);
        }
    }
}
