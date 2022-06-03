using Newtonsoft.Json;

namespace MovieBot.Models
{
    public class FilmResponse
    {
        [JsonProperty("trailer")]
        public Frame Trailer { get; set; }
        
        [JsonProperty("torrent")]
        public Frame Torrent { get; set; }

        [JsonProperty("videospider")]
        public Frame Videospider { get; set; }

        [JsonProperty("pleer")]
        public Frame Pleer { get; set; }

        [JsonProperty("kodik")]
        public Frame Kodik { get; set; }

        [JsonProperty("hdvb")]
        public Frame Hdvb { get; set; }

        [JsonProperty("videocdn")]
        public Frame Videocdn { get; set; }

        [JsonProperty("collaps")]
        public Frame Collaps { get; set; }

        [JsonProperty("iframe")]
        public Frame Iframe { get; set; }

        [JsonProperty("bazon")]
        public Frame Bazon { get; set; }

        [JsonProperty("ustore")]
        public Frame Ustore { get; set; }

        [JsonProperty("alloha")]
        public Frame Alloha { get; set; }
    }
}
