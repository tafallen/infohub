using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NewsAPI;
using NewsAPI.Models;
using NewsAPI.Constants;
using System.Collections.Generic;

namespace uk.me.timallen.infohub
{
    public static class GetNews
    {
        [FunctionName("GetNews")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var articles = GetArticles();
            string result = FormatResponse(articles);
            return new OkObjectResult(result.Substring(0, result.Length-1));
        }

        public static string FormatResponse(IList<Article> articles)
        {
            string result = "[";

            foreach (var article in articles)
            {
                result += $"{{\"title\":\"{article.Title}\",";
                result += $"\"author\":\"{article.Author}\",";
                result += $"\"description\":\"{article.Description}\",";
                result += $"\"publicationDate\":\"{article.PublishedAt}\"}},";
            }
            result += "]";
            return result;
        }

        public static IList<Article> GetArticles()
        {
            var newsKey = Environment.GetEnvironmentVariable("news_key");
            var newsApiClient = new NewsApiClient(newsKey);
            var sources = new List<string>(new []{"bbc-news"});

            var articlesResponse = newsApiClient.GetTopHeadlines( new TopHeadlinesRequest
            {
                Sources = sources,
                Language = Languages.EN,
                PageSize = 10
            });

            if (articlesResponse.Status == Statuses.Ok)
            {   
                return articlesResponse.Articles;
            }
            return new List<Article>();            
        }
    }
}
