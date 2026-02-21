using System;
using NewsAPI;
using NewsAPI.Models;
using NewsAPI.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

namespace uk.me.timallen.infohub
{
    public class NewsService : INewsService
    {
        private readonly INewsClientWrapper _client;
        private IList<Article>? _cachedArticles;
        private DateTime _lastFetchTime;
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);

        public NewsService(INewsClientWrapper client)
        {
            _client = client;
        }

        public async Task<string> GetNewsAsync()
        {
            var articles = await GetArticlesAsync();
            string result = FormatResponse(articles);
            return result;
        }

        private async Task<IList<Article>> GetArticlesAsync()
        {
            if (_cachedArticles != null && DateTime.UtcNow - _lastFetchTime < _cacheDuration)
            {
                return _cachedArticles;
            }

            await _cacheLock.WaitAsync();
            try
            {
                if (_cachedArticles != null && DateTime.UtcNow - _lastFetchTime < _cacheDuration)
                {
                    return _cachedArticles;
                }

                var sources = new List<string>(new[] { "bbc-news" });

                var articlesResponse = await _client.GetTopHeadlinesAsync(new TopHeadlinesRequest
                {
                    Sources = sources,
                    Language = Languages.EN,
                    PageSize = 10
                });

                if (articlesResponse.Status == Statuses.Ok)
                {
                    _cachedArticles = articlesResponse.Articles;
                    _lastFetchTime = DateTime.UtcNow;
                    return _cachedArticles;
                }
                return new List<Article>();
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        private string FormatResponse(IList<Article> articles)
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
                sb.Clear();
            }

            sb.Append("]");
            return sb.ToString();
        }

        private void FormatArticle(StringBuilder sb, Article article)
        {
            sb.Append($"{{\"title\":\"{article.Title}\",");
            sb.Append($"\"author\":\"{article.Author}\",");
            // Original code didn't check for null, but it's safer to do so.
            // If Description is null, Replace throws.
            // I'll add a check.
            string desc = article.Description != null ? article.Description.Replace('\"', '\'') : "";
            sb.Append($"\"description\":\"{desc}\",");
            sb.Append($"\"publicationDate\":\"{article.PublishedAt}\"}},");
        }
    }
}
