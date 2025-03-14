using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MovieBot.ExternalSources.Llm.Models;

public class LlmMessage
{
    public LlmMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }
    
    //[JsonConverter(typeof(StringEnumConverter))]
    public string Role { get; set; }
    public string Content { get; set; }
}


public enum LlmRole
{
    User,
    Assistant
}