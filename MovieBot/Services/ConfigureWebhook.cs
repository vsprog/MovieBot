using Microsoft.Extensions.Options;
using MovieBot.Infractructure;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MovieBot.Services;

public class ConfigureWebhook : BackgroundService
{ 
    private readonly IServiceProvider _serviceProvider;
    private readonly TelegramConfiguration _botConfig;

    public ConfigureWebhook(
        IServiceProvider serviceProvider,
        IOptions<TelegramConfiguration> botOptions)
    {
        _serviceProvider = serviceProvider;
        _botConfig = botOptions.Value;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        
        await botClient.SetWebhookAsync(
            url: $"{_botConfig.HostAddress}/telegram/respond",
            allowedUpdates: new []
            {
                UpdateType.Message, 
                UpdateType.EditedMessage
            },
            secretToken: _botConfig.SecretToken,
            cancellationToken: stoppingToken);
    }
}
