using Xunit;
using Moq;
using uk.me.timallen.infohub;
using RestSharp;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace infohub.tests
{
    public class HiveHeatingServiceTests
    {
        [Fact]
        public async Task GetHeatingStateAsync_ReturnsState_WhenCallsSucceed()
        {
            // Arrange
            var mockFactory = new Mock<IRestClientFactory>();
            var mockAuthClient = new Mock<IRestClient>();
            var mockNodeClient = new Mock<IRestClient>();

            // Setup Auth Call
            var authResponse = new Mock<IRestResponse>();
            // The service expects { "sessions": [ { "sessionId": "..." } ] }
            authResponse.Setup(r => r.Content).Returns("{ \"sessions\": [ { \"sessionId\": \"test-session-id\" } ] }");
            mockAuthClient.Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(authResponse.Object);

            // Setup Node Call
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

            // Use pattern matching for creating clients
            mockFactory.Setup(f => f.Create(It.Is<string>(s => s.Contains("auth/sessions")))).Returns(mockAuthClient.Object);
            // The node URL likely doesn't contain auth/sessions
            mockFactory.Setup(f => f.Create(It.Is<string>(s => !s.Contains("auth/sessions")))).Returns(mockNodeClient.Object);

            var service = new HiveHeatingService(mockFactory.Object);

            // Act
            var result = await service.GetHeatingStateAsync();

            // Assert
            Assert.Equal("MyThermostat", result.Name);
            Assert.Equal("ON", result.Heat);
            Assert.Equal("HEAT", result.Mode);
            // Note: 20.0 might be "20.0" or "20".
            // 20.0 in JSON is number. JToken conversion to string should be "20.0".
            // But let's verify loosely or check exactly if we know behaviour.
            Assert.Equal("20", result.Target);
        }
    }
}
