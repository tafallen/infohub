using Xunit;
using Moq;
using uk.me.timallen.infohub;
using NewsAPI.Models;
using NewsAPI.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace infohub.tests
{
    public class NewsServicePerformanceTests
    {
        [Fact]
        public async Task GetNewsAsync_CachingBenchmark()
        {
            // Arrange
            var mockClient = new Mock<INewsClientWrapper>();
            mockClient.Setup(c => c.GetTopHeadlinesAsync(It.IsAny<TopHeadlinesRequest>()))
                .Returns(async () =>
                {
                    await Task.Delay(100); // Simulate network delay
                    return new ArticlesResult
                    {
                        Status = Statuses.Ok,
                        Articles = new List<Article>
                        {
                            new Article { Title = "Test Title", PublishedAt = System.DateTime.Now }
                        }
                    };
                });

            var service = new NewsService(mockClient.Object);

            // Act - First Call (Cache Miss)
            var sw = Stopwatch.StartNew();
            await service.GetNewsAsync();
            sw.Stop();
            var firstCallTime = sw.ElapsedMilliseconds;

            // Act - Second Call (Should be cached)
            sw.Restart();
            await service.GetNewsAsync();
            sw.Stop();
            var secondCallTime = sw.ElapsedMilliseconds;

            // Output results
            System.Console.WriteLine($"First Call: {firstCallTime}ms");
            System.Console.WriteLine($"Second Call: {secondCallTime}ms");
        }
    }
}
