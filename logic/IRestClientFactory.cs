using RestSharp;

namespace uk.me.timallen.infohub
{
    public interface IRestClientFactory
    {
        IRestClient Create(string baseUrl);
    }
}
