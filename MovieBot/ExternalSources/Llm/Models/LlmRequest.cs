namespace MovieBot.ExternalSources.Llm.Models;

public class LlmRequest
{
    public string Model { get; set; } = null!;
    
    public LlmMessage[] Messages { get; set; } = null!;
}
