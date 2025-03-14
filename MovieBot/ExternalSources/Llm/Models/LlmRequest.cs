namespace MovieBot.ExternalSources.Llm.Models;

public class LlmRequest
{
    public string Model { get; set; } = null!;
    
    public int Temperature { get; set; }
    
    public List<LlmMessage> Messages { get; set; } = null!;
}
