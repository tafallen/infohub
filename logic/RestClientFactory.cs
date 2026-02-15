using RestSharp;

namespace uk.me.timallen.infohub
{
    public class RestClientFactory : IRestClientFactory
    {
        public IRestClient Create(string baseUrl)
        {
            var client = new RestClient(baseUrl);
            client.Timeout = -1;
            return client;
        }
    }
}
