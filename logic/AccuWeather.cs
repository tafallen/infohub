using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading;

namespace uk.me.timallen.infohub
{
    public static class AccuWeather
    {
        public static async Task<string> GetForecastAsync(string location, ILogger log)
        {
            var response = await MakeRequestAsync(location);
            log.LogInformation(response.Content);

            dynamic weather = JsonConvert.DeserializeObject(response.Content);
            if (weather == null || weather["DailyForecasts"] == null)
            {
                // Basic error handling or pass through, replicating original behavior which might have thrown if null
                return "";
            }
            return FormatResponse(weather["DailyForecasts"]);
        }

        private static string FormatResponse(dynamic dailyForecasts)
        {
            var forecasts = new List<object>();

            for(int i = 0; i < dailyForecasts.Count; i++)
            {
                dynamic day = dailyForecasts[i];
                forecasts.Add(new {
                    day = DateTime.Now.AddDays(i).DayOfWeek.ToString(),
                    min = day.Temperature.Minimum.Value.ToString(),
                    max = day.Temperature.Maximum.Value.ToString(),
                    summary = day.Day.IconPhrase.ToString(),
                    dayIcon = day.Day.Icon.ToString(),
                    nightIcon = day.Night.Icon.ToString()
                });
            }
            return JsonConvert.SerializeObject(forecasts);
        }

        private static async Task<IRestResponse> MakeRequestAsync(string location)
        {
            var client = GetRestClient(location);
            var request = new RestRequest(Method.GET);
            // RestSharp 106.11.4 specific: ExecuteAsync
            // To avoid the "obsolete" warning for ExecuteTaskAsync, use ExecuteAsync(request, cancellationToken)
            // But 106.11.4 documentation says ExecuteAsync returns Task<IRestResponse>.
            return await client.ExecuteAsync(request, CancellationToken.None);
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
