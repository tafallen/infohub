using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using RestSharp;
using uk.me.timallen.infohub;
using System.Threading;

namespace infohub.tests
{
    public class HiveHeatingServicePerformanceTests
    {
        [Fact]
        public async Task GetHeatingStateAsync_CachesSessionId_WhenCalledMultipleTimes()
        {
            // Arrange
            var mockFactory = new Mock<IRestClientFactory>();
            var mockAuthClient = new Mock<IRestClient>();
            var mockNodeClient = new Mock<IRestClient>();

            // Setup Auth Client
            var authResponse = new Mock<IRestResponse>();
            authResponse.Setup(r => r.Content).Returns("{ \"sessions\": [ { \"sessionId\": \"test-session-id\" } ] }");
            mockAuthClient.Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(authResponse.Object);

            // Setup Node Client
            var nodeResponse = new Mock<IRestResponse>();
            var nodeJson = @"{
                'nodes': [
                    {
                        'name': 'MyThermostat',
                        'id': '123',
                        'href': 'http://test',
                        'attributes': {
                            'stateHeatingRelay': { 'reportedValue': 'ON' },
                            'activeHeatCoolMode': { 'displayValue': 'HEAT' },
                            'targetHeatTemperature': { 'displayValue': 20.0 },
                            'temperature': { 'displayValue': 18.5 }
                        }
                    }
                ]
            }";
            nodeResponse.Setup(r => r.Content).Returns(nodeJson);
            mockNodeClient.Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(nodeResponse.Object);

            // Configure factory to return specific clients based on URL
            mockFactory.Setup(f => f.Create(It.Is<string>(s => s.Contains("auth/sessions")))).Returns(mockAuthClient.Object);
            mockFactory.Setup(f => f.Create(It.Is<string>(s => !s.Contains("auth/sessions")))).Returns(mockNodeClient.Object);

            var service = new HiveHeatingService(mockFactory.Object);

            // Act - First Call
            await service.GetHeatingStateAsync();

            // Act - Second Call (Should use cached session ID)
            await service.GetHeatingStateAsync();

            // Assert
            // Verify Auth client was called exactly ONCE
            mockAuthClient.Verify(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            // Verify Node client was called TWICE
            mockNodeClient.Verify(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
