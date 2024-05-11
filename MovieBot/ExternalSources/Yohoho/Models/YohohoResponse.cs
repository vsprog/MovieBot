using Newtonsoft.Json;

namespace MovieBot.ExternalSources.Yohoho.Models
{
    public class YohohoResponse
    {
        [JsonProperty("trailer")]
        public Frame Trailer { get; init; }
        
        [JsonProperty("torrent")]
        public Frame Torrent { get; init; }

        [JsonProperty("videospider")]
        public Frame Videospider { get; init; }

        [JsonProperty("pleer")]
        public Frame Pleer { get; init; }

        [JsonProperty("kodik")]
        public Frame Kodik { get; init; }

        [JsonProperty("hdvb")]
        public Frame Hdvb { get; init; }

        [JsonProperty("videocdn")]
        public Frame Videocdn { get; init; }

        [JsonProperty("collaps")]
        public Frame Collaps { get; init; }

        [JsonProperty("iframe")]
        public Frame Iframe { get; init; }

        [JsonProperty("bazon")]
        public Frame Bazon { get; init; }

        [JsonProperty("ustore")]
        public Frame Ustore { get; init; }

        [JsonProperty("alloha")]
        public Frame Alloha { get; init; }
    }
}
