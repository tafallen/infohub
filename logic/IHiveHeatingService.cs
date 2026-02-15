using System.Threading.Tasks;

namespace uk.me.timallen.infohub
{
    public interface IHiveHeatingService
    {
        Task<ThermostatState> GetHeatingStateAsync();
    }
}
