using Newtonsoft.Json;

namespace MovieBot.ExternalSources.MovieLab.Models;

public class LabFilm
{
    [JsonProperty("kinopoisk_id")]
    public string KinopoiskId { get; init; }
    
    [JsonProperty("title_ru")]
    public string TitleRu { get; init; }
    
    [JsonProperty("title_en")]
    public string TitleEn { get; init; }
    
    [JsonProperty("description")]
    public string Description { get; init; }
    
    [JsonProperty("player")]
    public LabPlayer Player { get; init; }
}
