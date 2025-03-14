using System.Text.Json.Serialization;

namespace MovieBot.ExternalSources.Llm.Models;

public class LlmMessage
{
    public LlmMessage(LlmRole role, string content)
    {
        Role = role;
        Content = content;
    }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LlmRole Role { get; set; }
    public string Content { get; set; }
}


public enum LlmRole
{
    User,
    Assistant
}