using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace uk.me.timallen.infohub
{
    public static class OpenWeather
    {
        public static async Task<string> GetForecastAsync(string lat, string lng)
        {
            var response = await MakeRequestAsync(lat,lng);
            dynamic weather = JsonConvert.DeserializeObject(response.Content);
            var dailyForecasts = weather["daily"];
            return FormatResponse(dailyForecasts);
        }

        private static async Task<IRestResponse> MakeRequestAsync(string lat, string lng)
        {
            var client = GetRestClient(lat, lng);
            var request = new RestRequest(Method.GET);
            return await client.ExecuteAsync(request, CancellationToken.None);
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
                    "\", \"wind_speed\": \"" + day.wind_speed + 
                    "\", \"wind_deg\": \"" + day.wind_deg + 
                    "\", \"sunrise\": \"" + day.sunrise + 
                    "\", \"sunset\": \"" + day.sunset + 
                    "\"},";
                result += forecast;
            }
            result = result.Substring(0, result.Length-1);
            return $"[{result}]";
        }
    }
}
