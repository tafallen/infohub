using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace uk.me.timallen.infohub
{
    public class GetWeather
    {
        private readonly IAccuWeatherService _weatherService;
        private readonly ILogger _logger;

        public GetWeather(IAccuWeatherService weatherService, ILoggerFactory loggerFactory)
        {
            _weatherService = weatherService;
            _logger = loggerFactory.CreateLogger<GetWeather>();
        }

        [Function("GetWeather")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req)
        {
            string location = req.Query["location"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (string.IsNullOrEmpty(location))
            {
                location = data?.location;
            }

            _logger.LogInformation("location: " + location);

            var response = await _weatherService.GetForecastAsync(location);
            _logger.LogInformation(response);

            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            httpResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
            httpResponse.WriteString(response);
            return httpResponse;
        }
    }
}
