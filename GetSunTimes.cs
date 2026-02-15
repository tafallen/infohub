using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace uk.me.timallen.infohub
{
    public class GetSunTimes
    {
        private readonly ISunriseSunsetService _sunService;
        private readonly ILogger _logger;

        public GetSunTimes(ISunriseSunsetService sunService, ILoggerFactory loggerFactory)
        {
            _sunService = sunService;
            _logger = loggerFactory.CreateLogger<GetSunTimes>();
        }

        [Function("GetSunTimes")]
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

            var result = await _sunService.GetSunriseSunsetTimesAsync(lat, lng);

            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            httpResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
            httpResponse.WriteString(result);
            return httpResponse;
        }
    }
}
