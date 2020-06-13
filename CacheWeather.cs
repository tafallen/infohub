using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace uk.me.timallen.infohub
{
    public static class CacheWeather
    {
        public class Weather_Forecast
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string Forecast { get; set; }
        }

        [FunctionName("CacheWeather")]
        [return: Table("weather")]
        public static Weather_Forecast Run([TimerTrigger("* 0 */6 * * *")]TimerInfo myTimer, ILogger log)
        {
            var lat = Environment.GetEnvironmentVariable("weatherlat");
            var lng = Environment.GetEnvironmentVariable("weatherlng");

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var forecast = OpenWeather.GetForecast(lat, lng);

            log.LogInformation(forecast);

            return new Weather_Forecast 
            { 
                PartitionKey = lat + "," + lng,
                RowKey = Guid.NewGuid().ToString(), 
                Forecast = forecast 
            };
        }
    }
}
