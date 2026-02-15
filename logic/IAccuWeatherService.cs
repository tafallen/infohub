using System.Threading.Tasks;

namespace uk.me.timallen.infohub
{
    public interface IAccuWeatherService
    {
        Task<string> GetForecastAsync(string location);
    }
}
