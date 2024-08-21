using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergiOverblikApp.Models
{
    public class TokenResponse
    {
        [JsonProperty("result")]
        public string AccessToken { get; set; }
        public string refresh_token { get; set; }
    }
}
