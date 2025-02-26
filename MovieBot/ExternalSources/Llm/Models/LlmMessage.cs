namespace MovieBot.ExternalSources.Llm.Models;

public class LlmMessage
{
    public LlmMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }
    
    public string Role { get; set; }
    public string Content { get; set; }
}
