using System.Text.Json.Serialization;

namespace MovieBot.ExternalSources.Llm.Models;

public class LlmRequest
{
    public string Model { get; set; } = null!;
    
    public int? Temperature { get; set; }
    
    [JsonPropertyName("top_p")]
    public int? Top { get; set; }
    
    public bool? Stream { get; set; }
    
    public LlmMessage[] Messages { get; set; } = null!;
}
