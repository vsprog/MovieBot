using Newtonsoft.Json;

namespace MovieBot.Models
{
    public class TokenResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("validBefore")]
        public long ValidBeforeMs { get; set; }
    }
}
