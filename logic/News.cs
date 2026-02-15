using System;
using NewsAPI;
using NewsAPI.Models;
using NewsAPI.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace uk.me.timallen.infohub
{
    public static class News
    {

        public static async Task<string> GetNewsAsync()
        {
            var articles = await GetArticlesAsync();
            string result = FormatResponse(articles);
            return result;
        }

        private static string FormatResponse(IList<Article> articles)
        {
            string result = "[";

            foreach (var article in articles)
            {
                result += FormatArticle(article);
            }

            result = result.Substring(0, result.Length-1);
            result += "]";
            return result;
        }

        private static string FormatArticle(Article article)
        {
            var result = string.Empty;
            result += $"{{\"title\":\"{article.Title}\",";
            result += $"\"author\":\"{article.Author}\",";
            result += $"\"description\":\"{article.Description.Replace('\"', '\'')}\",";
            result += $"\"publicationDate\":\"{article.PublishedAt}\"}},";
            return result;
        }

        private static async Task<IList<Article>> GetArticlesAsync()
        {
            var newsKey = Environment.GetEnvironmentVariable("news_key");
            var newsApiClient = new NewsApiClient(newsKey);
            var sources = new List<string>(new []{"bbc-news"});

            var articlesResponse = await newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
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
