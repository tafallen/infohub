using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading;

namespace uk.me.timallen.infohub
{
    public class AccuWeatherService : IAccuWeatherService
    {
        private readonly IRestClientFactory _clientFactory;
        private readonly ILogger<AccuWeatherService> _logger;

        public AccuWeatherService(IRestClientFactory clientFactory, ILogger<AccuWeatherService> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<string> GetForecastAsync(string location)
        {
            var response = await MakeRequestAsync(location);
            _logger.LogInformation(response.Content);

            dynamic weather = JsonConvert.DeserializeObject(response.Content);
            if (weather == null || weather["DailyForecasts"] == null)
            {
                return "";
            }
            return FormatResponse(weather["DailyForecasts"]);
        }

        private async Task<IRestResponse> MakeRequestAsync(string location)
        {
            var client = GetRestClient(location);
            var request = new RestRequest(Method.GET);
            return await client.ExecuteAsync(request, CancellationToken.None);
        }

        private IRestClient GetRestClient(string location)
        {
            var serviceApi = Environment.GetEnvironmentVariable("weatherapi");
            var serviceKey = Environment.GetEnvironmentVariable("weatherapikey");
            string url = serviceApi + location +"?apikey="+ serviceKey +"&details=false&metric=true";
            return _clientFactory.Create(url);
        }

        private string FormatResponse(dynamic dailyForecasts)
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
    }
}
