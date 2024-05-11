using Newtonsoft.Json;

namespace MovieBot.ExternalSources.MovieLab.Models;

public class LabPlayer
{
    [JsonProperty("iframe_url")]
    public string Url { get; init; }

    [JsonProperty("quality")]
    public string Quality { get; init; }

    [JsonProperty("translator")]
    public string Translate { get; init; }
}
