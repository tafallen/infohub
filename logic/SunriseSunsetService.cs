using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace uk.me.timallen.infohub
{
    public class SunriseSunsetService : ISunriseSunsetService
    {
        private readonly IRestClientFactory _clientFactory;

        public SunriseSunsetService(IRestClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetSunriseSunsetTimesAsync(string lat, string lng)
        {
            var serviceUrl ="https://api.sunrise-sunset.org/json" + "?lat=" + lat + "&lng=" + lng;
            var client = _clientFactory.Create(serviceUrl);
            var request = new RestRequest(Method.GET);

            var response = await client.ExecuteAsync(request, CancellationToken.None);
            return FormatResponse(response);
        }

        private string FormatResponse(IRestResponse response)
        {
            dynamic sunTimes = JsonConvert.DeserializeObject(response.Content);

            return "{\"sunrise\":\"" +
                (string)sunTimes["results"]["sunrise"] +
                "\", \"sunset\":\"" +
                (string)sunTimes["results"]["sunset"] +
                "\"}";
        }
    }
}
