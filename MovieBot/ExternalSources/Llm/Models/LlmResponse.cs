namespace MovieBot.ExternalSources.Llm.Models;

public class LlmResponse
{
    public Choice[] Choices { get; set; } = null!;
}

public class Choice
{
    public int Index { get; set; }
    public LlmMessage Message { get; set; } = null!;
}
