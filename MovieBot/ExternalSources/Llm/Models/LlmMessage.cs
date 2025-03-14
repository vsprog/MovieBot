using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace MovieBot.ExternalSources.Llm.Models;

public class LlmMessage
{
    public LlmMessage(LlmRole role, string content)
    {
        Role = role;
        Content = content;
    }
    
    
    [JsonConverter(typeof(StringEnumConverter))]
    public LlmRole Role { get; set; }
    public string Content { get; set; }
}


public enum LlmRole
{
    User,
    Assistant
}