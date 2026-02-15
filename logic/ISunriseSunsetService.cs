using System.Threading.Tasks;

namespace uk.me.timallen.infohub
{
    public interface ISunriseSunsetService
    {
        Task<string> GetSunriseSunsetTimesAsync(string lat, string lng);
    }
}
