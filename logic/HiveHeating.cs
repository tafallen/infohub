using System;
using Newtonsoft.Json;
using RestSharp;

namespace uk.me.timallen.infohub
{
    public class HiveHeating
    {
        public static ThermostatState GetHeatingState()
        {
            var instance = new HiveHeating();
            return instance.GetThermostatState();
        }

        public ThermostatState GetThermostatState()
        {
            var client = new RestClient(GetHiveNodeThermostatUrl())
            {
                Timeout = -1
            };

            var request = GetRequest(Method.GET);
            request.AddHeader("X-Omnia-Access-Token", GetSessionId());

            IRestResponse response = client.Execute(request);
            dynamic heating = JsonConvert.DeserializeObject(response.Content);
            return MapToThermostatState(heating["nodes"][0]);
        }

        private string GetSessionId()
        {
            var client = new RestClient(GetHiveAuthUrl())
            {
                Timeout = -1
            };

            var request = GetRequest(Method.POST);
            request.AddParameter("application/vnd.alertme.zoo-6.1+json", 
                                    "{\r\n\"sessions\": [{\r\n\"username\": \"" + 
                                    GetUsername() + 
                                    "\",\r\n\"password\": \"" + 
                                    GetPassword() + 
                                    "\",\r\n\"caller\": \"WEB\"\r\n}]\r\n}",  
                                    ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
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

        private RestRequest GetRequest(RestSharp.Method method)
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
            return "https://api.prod.bgchprod.info:443/omnia";
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