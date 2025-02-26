namespace MovieBot.Infractructure;

public class LlmConfiguration
{
    public static readonly string Configuration = "Llm";
    
    public string Model { get; init; } = null!;
}
