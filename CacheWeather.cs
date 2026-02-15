using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace uk.me.timallen.infohub
{
    public class CacheWeather
    {
        private readonly IOpenWeatherService _weatherService;
        private readonly ILogger _logger;

        public CacheWeather(IOpenWeatherService weatherService, ILoggerFactory loggerFactory)
        {
            _weatherService = weatherService;
            _logger = loggerFactory.CreateLogger<CacheWeather>();
        }

        public class WeatherForecast
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string Forecast { get; set; }
        }

        [Function("CacheWeather")]
        [TableOutput("weather")]
        public async Task<WeatherForecast> Run([TimerTrigger("* 0 */6 * * *")] TimerInfo myTimer)
        {
            var lat = Environment.GetEnvironmentVariable("weatherlat");
            var lng = Environment.GetEnvironmentVariable("weatherlng");

            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var forecast = await _weatherService.GetForecastAsync(lat, lng);

            _logger.LogInformation(forecast);

            return new WeatherForecast 
            { 
                PartitionKey = lat + "," + lng,
                RowKey = Guid.NewGuid().ToString(), 
                Forecast = forecast 
            };
        }
    }
}
