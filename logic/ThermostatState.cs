using Newtonsoft.Json;

namespace uk.me.timallen.infohub
{
    public class ThermostatState
    {
        [JsonProperty("heat")]
        public string Heat {get; set;}

        [JsonProperty("mode")]
        public string Mode {get; set;}

        [JsonProperty("target")]
        public string Target {get; set;}

        [JsonProperty("temperature")]
        public string Temperature {get; set;}

        [JsonProperty("name")]
        public string Name {get; set;}

        [JsonProperty("id")]
        public string Id {get; set;}

        [JsonProperty("href")]
        public string Href {get; set;}

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}