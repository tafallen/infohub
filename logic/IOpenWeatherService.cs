using System.Threading.Tasks;

namespace uk.me.timallen.infohub
{
    public interface IOpenWeatherService
    {
        Task<string> GetForecastAsync(string lat, string lng);
    }
}
