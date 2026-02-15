using Xunit;
using Moq;
using uk.me.timallen.infohub;
using NewsAPI.Models;
using NewsAPI.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace infohub.tests
{
    public class NewsServiceTests
    {
        [Fact]
        public async Task GetNewsAsync_ReturnsLegacyEmptyBracket_WhenNoArticles()
        {
            // Arrange
            var mockClient = new Mock<INewsClientWrapper>();
            mockClient.Setup(c => c.GetTopHeadlinesAsync(It.IsAny<TopHeadlinesRequest>()))
                .ReturnsAsync(new ArticlesResult { Status = Statuses.Ok, Articles = new List<Article>() });

            var service = new NewsService(mockClient.Object);

            // Act
            var result = await service.GetNewsAsync();

            // Assert
            // Legacy behavior: returns "]" when empty
            Assert.Equal("]", result);
        }

        [Fact]
        public async Task GetNewsAsync_ReturnsFormattedJson_WhenArticlesExist()
        {
            // Arrange
            var articles = new List<Article>
            {
                new Article {
                    Title = "Test Title",
                    Author = "Test Author",
                    Description = "Test \"Description\"", // Quote inside
                    PublishedAt = new System.DateTime(2023, 1, 1)
                }
            };
            var mockClient = new Mock<INewsClientWrapper>();
            mockClient.Setup(c => c.GetTopHeadlinesAsync(It.IsAny<TopHeadlinesRequest>()))
                .ReturnsAsync(new ArticlesResult { Status = Statuses.Ok, Articles = articles });

            var service = new NewsService(mockClient.Object);

            // Act
            var result = await service.GetNewsAsync();

            // Assert
            Assert.Contains("\"title\":\"Test Title\"", result);
            Assert.Contains("\"author\":\"Test Author\"", result);
            Assert.Contains("\"description\":\"Test 'Description'\"", result); // Quote replaced
            Assert.StartsWith("[", result);
            Assert.EndsWith("]", result);
        }
    }
}
