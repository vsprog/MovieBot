namespace MovieBot.Infrastructure.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var botConfigurationSection = configuration.GetSection(TelegramConfiguration.Configuration);
        services.Configure<TelegramConfiguration>(botConfigurationSection);
        var llmConfiguration = configuration.GetSection(LlmConfiguration.Configuration);
        services.Configure<LlmConfiguration>(llmConfiguration);
    }
}
