using Newtonsoft.Json;

namespace EnergiOverblikApp.Models
{
    public class TokenResponse
    {
        [JsonProperty("result")]
        public string AccessToken { get; set; }
        public string refresh_token { get; set; }
    }
}
