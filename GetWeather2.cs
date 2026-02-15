using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace uk.me.timallen.infohub
{
    public class GetWeather2
    {
        private readonly IOpenWeatherService _weatherService;
        private readonly ILogger _logger;

        public GetWeather2(IOpenWeatherService weatherService, ILoggerFactory loggerFactory)
        {
            _weatherService = weatherService;
            _logger = loggerFactory.CreateLogger<GetWeather2>();
        }

        [Function("GetWeather2")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req)
        {
            string lat = req.Query["lat"];
            string lng = req.Query["lng"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (string.IsNullOrEmpty(lat)) lat = data?.lat;
            if (string.IsNullOrEmpty(lng)) lng = data?.lng;

            _logger.LogInformation("lat: " + lat + " lng:" + lng);

            var response = await _weatherService.GetForecastAsync(lat, lng);

            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            httpResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
            httpResponse.WriteString(response);
            return httpResponse;
        }
    }
}
