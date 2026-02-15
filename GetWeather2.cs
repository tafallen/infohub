using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

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

            // var table = GetStorageTable("weather");
            // var result = GetMostRecentEntry<Weather>(table, lat + "," + lng).Forecast;
            var result = await OpenWeather.GetForecastAsync(lat, lng);
            return new OkObjectResult(result);
        }

        public static T GetMostRecentEntry<T>(CloudTable table, string partitionKey) where T : ITableEntity, new()
        {
            var query = new TableQuery<T>()
                .Where(
                    TableQuery.GenerateFilterCondition(
                        "PartitionKey", 
                        QueryComparisons.Equal, partitionKey));

            var items = table.ExecuteQuerySegmentedAsync(
                query, 
                new TableContinuationToken()
                ).Result.Results;

            return items.OrderByDescending(s => s.Timestamp).FirstOrDefault();
        }

        public static CloudTable GetStorageTable(string table)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var account = CloudStorageAccount.Parse(connectionString);
            var tableClient = account.CreateCloudTableClient();
            return tableClient.GetTableReference(table);
        }
    }
    class Weather : TableEntity
    {
        public string Forecast{ get; set; }
    }
}
