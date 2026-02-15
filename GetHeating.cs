using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace uk.me.timallen.infohub
{
    public class GetHeating
    {
        private readonly IHiveHeatingService _heatingService;
        private readonly ILogger _logger;

        public GetHeating(IHiveHeatingService heatingService, ILoggerFactory loggerFactory)
        {
            _heatingService = heatingService;
            _logger = loggerFactory.CreateLogger<GetHeating>();
        }

        [Function("GetHeating")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req)
        {
            var state = await _heatingService.GetHeatingStateAsync();
            var result = state.ToString();
            _logger.LogInformation(result);

            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            httpResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
            httpResponse.WriteString(result);
            return httpResponse;
        }
    }
}
