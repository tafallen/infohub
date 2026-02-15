using System.Threading.Tasks;

namespace uk.me.timallen.infohub
{
    public interface INewsService
    {
        Task<string> GetNewsAsync();
    }
}
