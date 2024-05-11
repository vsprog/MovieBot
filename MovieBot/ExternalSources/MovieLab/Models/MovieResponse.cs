using Newtonsoft.Json;

namespace MovieBot.ExternalSources.MovieLab.Models;

public class MovieResponse
{
    [JsonProperty("results")]
    public IEnumerable<LabFilm> Films { get; init; }
}
