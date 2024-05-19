namespace MovieBot.Infractructure;

public class TelegramConfiguration
{
    public static readonly string Configuration = "Telegram";
    
    public string HostAddress { get; init; } = default!;
    public string BotToken { get; init; } = default!;
    public string SecretToken { get; init; } = default!;
}
