using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace uk.me.timallen.infohub
{
    public static class GetWeather
    {
        [FunctionName("GetWeather")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string location = req.Query["location"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            location = location ?? data?.location;

            log.LogInformation("location: " + location);

            var response = await AccuWeather.GetForecastAsync(location, log);

            log.LogInformation(response);

            return new OkObjectResult(response);
        }
    }
}
