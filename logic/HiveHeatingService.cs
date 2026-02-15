using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace uk.me.timallen.infohub
{
    public class HiveHeatingService : IHiveHeatingService
    {
        private readonly IRestClientFactory _clientFactory;

        public HiveHeatingService(IRestClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<ThermostatState> GetHeatingStateAsync()
        {
            var client = _clientFactory.Create(GetHiveNodeThermostatUrl());
            var request = GetRequest(Method.GET);
            var sessionId = await GetSessionIdAsync();
            request.AddHeader("X-Omnia-Access-Token", sessionId);

            IRestResponse response = await client.ExecuteAsync(request);
            dynamic heating = JsonConvert.DeserializeObject(response.Content);
            return MapToThermostatState(heating["nodes"][0]);
        }

        private async Task<string> GetSessionIdAsync()
        {
            var client = _clientFactory.Create(GetHiveAuthUrl());

            var request = GetRequest(Method.POST);
            request.AddParameter("application/vnd.alertme.zoo-6.1+json", 
                                    "{\r\n\"sessions\": [{\r\n\"username\": \"" + 
                                    GetUsername() + 
                                    "\",\r\n\"password\": \"" + 
                                    GetPassword() + 
                                    "\",\r\n\"caller\": \"WEB\"\r\n}]\r\n}",  
                                    ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic credentials = JsonConvert.DeserializeObject(response.Content);
            return credentials["sessions"][0]["sessionId"];
        }

        private ThermostatState MapToThermostatState(dynamic thermo)
        {
            var result = new ThermostatState()
            {
                Heat = thermo["attributes"]["stateHeatingRelay"]["reportedValue"],
                Mode = thermo["attributes"]["activeHeatCoolMode"]["displayValue"],
                Target = thermo["attributes"]["targetHeatTemperature"]["displayValue"],
                Temperature = thermo["attributes"]["temperature"]["displayValue"],
                Name = thermo["name"],
                Id = thermo["id"],
                Href = thermo["href"]
            };

            return result;
        }

        private RestRequest GetRequest(Method method)
        {
            var request = new RestRequest(method);
            request.AddHeader("Content-Type", "application/vnd.alertme.zoo-6.1+json");
            request.AddHeader("Accept", "application/vnd.alertme.zoo-6.1+json");
            request.AddHeader("X-Omnia-Client", "Hive Web Dashboard");
            return request;
        }

        #region Get settings from application settings
        private string GetUsername()
        {
            return Environment.GetEnvironmentVariable("hive_username");
        }

        private string GetPassword()
        {
            return Environment.GetEnvironmentVariable("hive_password");
        }

        private string GetHiveBaseUrl()
        {
            var url = Environment.GetEnvironmentVariable("hive_base_url");
            return string.IsNullOrEmpty(url) ? "https://api.prod.bgchprod.info:443/omnia" : url;
        }

        private string GetHiveAuthUrl()
        {
            return GetHiveBaseUrl() + "/auth/sessions";
        }

        private string GetHiveNodeUrl()
        {
            return GetHiveBaseUrl() + "/nodes";
        }

        private string GetHiveNodeThermostatUrl()
        {
            return GetHiveNodeUrl() + "/" + 
            Environment.GetEnvironmentVariable("hive_thermo_node");
        }
        #endregion
    }
}
