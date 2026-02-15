using System;
using NewsAPI;
using NewsAPI.Models;
using NewsAPI.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

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
            var sb = new StringBuilder();
            sb.Append("[");

            foreach (var article in articles)
            {
                FormatArticle(sb, article);
            }

            if (sb.Length > 1)
            {
                sb.Length--; // Remove trailing comma
            }
            else
            {
                // Edge case: empty list
                // If the list is empty, sb is "[".
                // We want to return "]" to match original behavior which returned "]" for empty list.
                // Original: result="[", substring(0,0) -> "", + "]" -> "]"
                sb.Clear();
            }

            sb.Append("]");
            return sb.ToString();
        }

        private static void FormatArticle(StringBuilder sb, Article article)
        {
            sb.Append($"{{\"title\":\"{article.Title}\",");
            sb.Append($"\"author\":\"{article.Author}\",");
            sb.Append($"\"description\":\"{article.Description.Replace('\"', '\'')}\",");
            sb.Append($"\"publicationDate\":\"{article.PublishedAt}\"}},");
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
