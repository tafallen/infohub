using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json;

namespace uk.me.timallen.infohub
{
    public static class GetSunTimes
    {
        [FunctionName("GetSunTimes")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("In GetSunTimes");

            string lat = req.Query["lat"];
            string lng = req.Query["lng"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            lat = lat ?? data?.lat;
            lng = lng ?? data?.lng;

            log.LogInformation("lat: " + lat + " lng:" + lng);

            var client = new RestClient("https://api.sunrise-sunset.org/json?lat=" + lat + "&lng=" + lng);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            dynamic sunTimes = JsonConvert.DeserializeObject(response.Content);

            return new OkObjectResult(
                "{\"sunrise\":\"" + 
                sunTimes["results"]["sunrise"] + 
                "\", \"sunset\":\"" + 
                sunTimes["results"]["sunset"] + 
                "\"}");
        }
    }
}
