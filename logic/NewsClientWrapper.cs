using System;
using System.Threading.Tasks;
using NewsAPI;
using NewsAPI.Models;

namespace uk.me.timallen.infohub
{
    public class NewsClientWrapper : INewsClientWrapper
    {
        private readonly NewsApiClient _client;

        public NewsClientWrapper()
        {
            var newsKey = Environment.GetEnvironmentVariable("news_key");
            _client = new NewsApiClient(newsKey);
        }

        public async Task<ArticlesResult> GetTopHeadlinesAsync(TopHeadlinesRequest request)
        {
            return await _client.GetTopHeadlinesAsync(request);
        }
    }
}
