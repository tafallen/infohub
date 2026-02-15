using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace uk.me.timallen.infohub
{
    public class CacheNews
    {
        private readonly INewsService _newsService;
        private readonly ILogger _logger;

        public CacheNews(INewsService newsService, ILoggerFactory loggerFactory)
        {
            _newsService = newsService;
            _logger = loggerFactory.CreateLogger<CacheNews>();
        }

        [Function("CacheNews")]
        [TableOutput("news")]
        public async Task<NewsArticles> Run([TimerTrigger("* 0 */6 * * *")] TimerInfo myTimer)
        {
            var result = new NewsArticles
            {
                PartitionKey = "bbc-news",
                RowKey = Guid.NewGuid().ToString(),
                Articles = await _newsService.GetNewsAsync()
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
