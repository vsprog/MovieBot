namespace MovieBot.Infrastructure.Configurations;

public class TelegramConfiguration
{
    public static readonly string Configuration = "Telegram";
    
    public string HostAddress { get; init; } = null!;
    public string BotToken { get; init; } = null!;
    public string SecretToken { get; init; } = null!;
}
