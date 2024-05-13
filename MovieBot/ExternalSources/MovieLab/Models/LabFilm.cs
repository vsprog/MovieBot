using Newtonsoft.Json;

namespace MovieBot.ExternalSources.MovieLab.Models;

public class LabFilm
{
    [JsonProperty("kinopoisk_id")]
    public string KinopoiskId { get; init; }
    
    [JsonProperty("year")]
    public int Year { get; init; }
    
    [JsonProperty("type")]
    public string Type { get; init; }
    
    [JsonProperty("duration")]
    public int Duration { get; init; }
    
    [JsonProperty("title_ru")]
    public string TitleRu { get; init; }
    
    [JsonProperty("title_en")]
    public string TitleEn { get; init; }
    
    [JsonProperty("description")]
    public string Description { get; init; }
    
    [JsonProperty("poster")]
    public string PosterUrl { get; init; }
    
    [JsonProperty("player")]
    public LabPlayer Player { get; init; }
}
