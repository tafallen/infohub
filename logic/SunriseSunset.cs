using Newtonsoft.Json;
using RestSharp;

namespace uk.me.timallen.infohub
{
    public static class SunriseSunset
    {
        public static string GetSunriseSunsetTimes(string lat, string lng)
        {
            return GetFromSunriseSunset(lat, lng);
        }
        private static string GetFromSunriseSunset(string lat, string lng)
        {
            var serviceUrl ="https://api.sunrise-sunset.org/json" + "?lat=" + lat + "&lng=" + lng;
            var client = new RestClient(serviceUrl);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);

            return FormatResponse(client.Execute(request));
        }

        private static string FormatResponse(IRestResponse response)
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