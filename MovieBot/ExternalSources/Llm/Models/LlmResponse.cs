namespace MovieBot.ExternalSources.Llm.Models;

public class LlmResponse
{
    public Choise[] Choises { get; set; } = null!;
}

public class Choise
{
    public int Index { get; set; }
    public LlmMessage LlmMessage { get; set; } = null!;
}
