using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace uk.me.timallen.infohub
{
    public static class CacheWeather
    {
        public class WeatherForecast
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string Forecast { get; set; }
        }

        [FunctionName("CacheWeather")]
        [return: Table("weather")]
        public static async Task<WeatherForecast> Run([TimerTrigger("* 0 */6 * * *")]TimerInfo myTimer, ILogger log)
        {
            var lat = Environment.GetEnvironmentVariable("weatherlat");
            var lng = Environment.GetEnvironmentVariable("weatherlng");

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var forecast = await OpenWeather.GetForecastAsync(lat, lng);

            log.LogInformation(forecast);

            return new WeatherForecast 
            { 
                PartitionKey = lat + "," + lng,
                RowKey = Guid.NewGuid().ToString(), 
                Forecast = forecast 
            };
        }
    }
}
