using Newtonsoft.Json;

namespace MovieBot.ExternalSources.Yohoho.Models
{
    public class Frame
    {
        [JsonProperty("iframe")]
        public string Url { get; init; }

        [JsonProperty("quality")]
        public string Quality { get; init; }

        [JsonProperty("translate")]
        public string Translate { get; init; }
    }
}
