using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace uk.me.timallen.infohub
{
    public class OpenWeatherService : IOpenWeatherService
    {
        private readonly IRestClientFactory _clientFactory;

        public OpenWeatherService(IRestClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetForecastAsync(string lat, string lng)
        {
            var response = await MakeRequestAsync(lat, lng);
            dynamic weather = JsonConvert.DeserializeObject(response.Content);
            var dailyForecasts = weather["daily"];
            return FormatResponse(dailyForecasts);
        }

        private async Task<IRestResponse> MakeRequestAsync(string lat, string lng)
        {
            var client = GetRestClient(lat, lng);
            var request = new RestRequest(Method.GET);
            return await client.ExecuteAsync(request, CancellationToken.None);
        }        

        private IRestClient GetRestClient(string lat, string lng)
        {
            var serviceApi = Environment.GetEnvironmentVariable("openweatherapiurl");
            var serviceKey = Environment.GetEnvironmentVariable("openweatherapikey");
            string url = serviceApi + "?lat=" + lat + "&lon=" + lng + "&exclude=hourly&units=metric&appid=" + serviceKey;
            return _clientFactory.Create(url);
        }

        private string FormatResponse(dynamic dailyForecasts)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            int count = dailyForecasts.Count;
            for( int i = 0; i < count; i++)
            {
                dynamic day = dailyForecasts[i];

                sb.Append("{ \"day\": \"");
                sb.Append(DateTime.Now.AddDays(i).DayOfWeek.ToString());
                sb.Append("\", \"min\": \"");
                sb.Append(day.temp.min);
                sb.Append("\", \"max\": \"");
                sb.Append(day.temp.max);
                sb.Append("\", \"summary\":\"");
                sb.Append(day.weather[0].main);
                sb.Append("\", \"dayIcon\": \"");
                sb.Append(day.weather[0].icon);
                sb.Append("\", \"nightIcon\": \"");
                sb.Append(day.weather[0].icon);
                sb.Append("\", \"wind_speed\": \"");
                sb.Append(day.wind_speed);
                sb.Append("\", \"wind_deg\": \"");
                sb.Append(day.wind_deg);
                sb.Append("\", \"sunrise\": \"");
                sb.Append(day.sunrise);
                sb.Append("\", \"sunset\": \"");
                sb.Append(day.sunset);
                sb.Append("\"}");

                if (i < count - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
