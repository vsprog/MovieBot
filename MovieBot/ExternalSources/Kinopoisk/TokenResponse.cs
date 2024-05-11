using Newtonsoft.Json;

namespace MovieBot.ExternalSources.Kinopoisk
{
    public class TokenResponse
    {
        [JsonProperty("token")]
        public string Token { get; init; }

        [JsonProperty("validBefore")]
        public long ValidBeforeMs { get; init; }
    }
}
