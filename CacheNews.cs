using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace uk.me.timallen.infohub
{
    public static class CacheNews
    {
        [FunctionName("CacheNews")]
        [return: Table("news")]
        public static NewsArticles Run([TimerTrigger("* 0 */6 * * *")]TimerInfo myTimer, ILogger log)
        //public static NewsArticles Run([TimerTrigger("* * * * * *")]TimerInfo myTimer, ILogger log)
        {
            var result = new NewsArticles
            {
                PartitionKey = "bbc-news",
                RowKey = Guid.NewGuid().ToString(),
                Articles = News.GetNews()
            };
            return result;
        }

        public class NewsArticles
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string Articles { get; set; }
        }
    }
}
