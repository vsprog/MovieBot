using Newtonsoft.Json;

namespace MovieBot.Models
{
    public class Frame
    {
        [JsonProperty("iframe")]
        public string Url { get; set; }

        [JsonProperty("quality")]
        public string Quality { get; set; }

        [JsonProperty("translate")]
        public string Translate { get; set; }
    }
}
