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

            var response = GetFromAccuweather(location);

            log.LogInformation(response);

            return new OkObjectResult(response);
        }

        private static string GetFromAccuweather(string location)
        {
            var response = MakeRequest(location);
            Console.WriteLine(response);
            dynamic weather = JsonConvert.DeserializeObject(response.Content);
            return FormatResponse(weather["DailyForecasts"]);
        }

        private static string FormatResponse(dynamic dailyForecasts)
        {
            string result = string.Empty;

            for(int i = 0; i < dailyForecasts.Count; i++)
            {
                dynamic day = dailyForecasts[i];
                var  forecast =  "{ \"day\": \"" + DateTime.Now.AddDays(i).DayOfWeek.ToString() + 
                    "\", \"min\": \"" + day.Temperature.Minimum.Value + 
                    "\", \"max\": \"" + day.Temperature.Maximum.Value + 
                    "\", \"summary\":\"" + day.Day.IconPhrase + 
                    "\", \"dayIcon\": \"" + day.Day.Icon + 
                    "\", \"nightIcon\": \"" + day.Night.Icon + 
                    "\"},";

                result += forecast;
            }
            result = result.Substring(0, result.Length-1);
            return $"[{result}]";
        }

        private static IRestResponse MakeRequest(string location)
        {
            var client = GetRestClient(location);
            var request = new RestRequest(Method.GET);
            return client.Execute(request);
        }

        private static RestClient GetRestClient(string location)
        {
            var serviceApi = Environment.GetEnvironmentVariable("weatherapi");
            var serviceKey = Environment.GetEnvironmentVariable("weatherapikey");
            return new RestClient(serviceApi + location +"?apikey="+ serviceKey +"&details=false&metric=true")
            {
                Timeout = -1
            };
        }
    }
}
