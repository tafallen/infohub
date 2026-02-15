using Xunit;
using Moq;
using uk.me.timallen.infohub;
using RestSharp;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace infohub.tests
{
    public class AccuWeatherServiceTests
    {
        [Fact]
        public async Task GetForecastAsync_ReturnsEmptyString_WhenResponseIsInvalid()
        {
            // Arrange
            var mockFactory = new Mock<IRestClientFactory>();
            var mockClient = new Mock<IRestClient>();
            var mockResponse = new Mock<IRestResponse>();
            var mockLogger = new Mock<ILogger<AccuWeatherService>>();

            mockResponse.Setup(r => r.Content).Returns("{}"); // No DailyForecasts
            mockClient.Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse.Object);
            mockFactory.Setup(f => f.Create(It.IsAny<string>())).Returns(mockClient.Object);

            var service = new AccuWeatherService(mockFactory.Object, mockLogger.Object);

            // Act
            var result = await service.GetForecastAsync("London");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public async Task GetForecastAsync_ReturnsFormattedJson_WhenResponseIsValid()
        {
            // Arrange
            var json = @"{
                'DailyForecasts': [
                    {
                        'Temperature': { 'Minimum': { 'Value': 10 }, 'Maximum': { 'Value': 20 } },
                        'Day': { 'IconPhrase': 'Sunny', 'Icon': 1 },
                        'Night': { 'Icon': 2 }
                    }
                ]
            }";

            var mockFactory = new Mock<IRestClientFactory>();
            var mockClient = new Mock<IRestClient>();
            var mockResponse = new Mock<IRestResponse>();
            var mockLogger = new Mock<ILogger<AccuWeatherService>>();

            mockResponse.Setup(r => r.Content).Returns(json);
            mockClient.Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse.Object);
            mockFactory.Setup(f => f.Create(It.IsAny<string>())).Returns(mockClient.Object);

            var service = new AccuWeatherService(mockFactory.Object, mockLogger.Object);

            // Act
            var result = await service.GetForecastAsync("London");

            // Assert
            Assert.Contains("\"min\":\"10\"", result);
            Assert.Contains("\"max\":\"20\"", result);
            Assert.Contains("\"summary\":\"Sunny\"", result);
        }
    }
}
