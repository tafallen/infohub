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

namespace uk.me.timallen.infohub
{
    public static class GetWeather2
    {
        [FunctionName("GetWeather2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string lat = req.Query["lat"];
            string lng = req.Query["lng"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            lat = lat ?? data?.lat;
            lng = lng ?? data?.lng;

            log.LogInformation("lat: " + lat + " lng:" + lng);

            var result = GetFromOpenWeather(lat, lng);

            return new OkObjectResult(result);
        }

        private static string GetFromOpenWeather(string lat, string lng)
        {
            var response = MakeRequest(lat,lng);
            dynamic weather = JsonConvert.DeserializeObject(response.Content);
            var dailyForecasts = weather["daily"];
            return FormatResponse(dailyForecasts);
        }

        private static IRestResponse MakeRequest(string lat, string lng)
        {
            var client = GetRestClient(lat, lng);
            var request = new RestRequest(Method.GET);
            return client.Execute(request);
        }        

        private static RestClient GetRestClient(string lat, string lng)
        {
            var serviceApi = Environment.GetEnvironmentVariable("openweatherapiurl");
            var serviceKey = Environment.GetEnvironmentVariable("openweatherapikey");
            return new RestClient(serviceApi + "?lat=" + lat + "&lon=" + lng + "&exclude=hourly&units=metric&appid=" + serviceKey)
            {
                Timeout = -1
            };
        }

        private static string FormatResponse(dynamic dailyForecasts)
        {
            string result = string.Empty;

            for( int i = 0; i < dailyForecasts.Count; i++)
            {
                dynamic day = dailyForecasts[i];

                var  forecast =  "{ \"day\": \"" + DateTime.Now.AddDays(i).DayOfWeek.ToString() + 
                    "\", \"min\": \"" + day.temp.min + 
                    "\", \"max\": \"" + day.temp.max + 
                    "\", \"summary\":\"" + day.weather[0].main + 
                    "\", \"dayIcon\": \"" + day.weather[0].icon + 
                    "\", \"nightIcon\": \"" + day.weather[0].icon + 
                    "\"},";

                result += forecast;
            }
            result = result.Substring(0, result.Length-1);
            result = $"[{result}]";

            return result;
        }
    }
}
