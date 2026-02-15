using System.Threading.Tasks;
using NewsAPI.Models;

namespace uk.me.timallen.infohub
{
    public interface INewsClientWrapper
    {
        Task<ArticlesResult> GetTopHeadlinesAsync(TopHeadlinesRequest request);
    }
}
