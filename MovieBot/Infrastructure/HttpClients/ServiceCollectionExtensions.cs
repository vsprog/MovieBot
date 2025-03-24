using System.Net;
using System.Net.Http.Headers;
using MovieBot.ExternalSources;
using MovieBot.ExternalSources.Kinopoisk;
using MovieBot.ExternalSources.Llm;
using MovieBot.ExternalSources.MovieLab;
using MovieBot.ExternalSources.Yohoho;
using Telegram.Bot;

namespace MovieBot.Infrastructure.HttpClients;

public static class ServiceCollectionExtensions
{
    private const string UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36";

    public static void AddHttpClients(this IServiceCollection services, IConfiguration config)
    {
        services.AddConfiguredClient<KinopoiskClient>(client =>
        {
            client.BaseAddress = new Uri(config["ContentSources:Kinopoisk"]!);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        });

        services.AddConfiguredClient<YohohoClient>(client =>
        {
            client.BaseAddress = new Uri(config["ContentSources:Yohoho"]!);
            client.DefaultRequestHeaders.Add("referer", config["ContentSources:YohoReferer"]);
        });
        
        services.AddConfiguredClient<MovieLabClient>(client =>
        {
            client.BaseAddress = new Uri(config["ContentSources:Lab"]!);
            client.DefaultRequestHeaders.Add("referer", config["ContentSources:LabReferer"]);
            client.DefaultRequestHeaders.Add("Origin", config["ContentSources:LabOrigin"]);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        });
        
        services.AddHttpClient<FileLoader>();
        
        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, _) =>
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                
                TelegramBotClientOptions options = new(config["Telegram:BotToken"]!);
                
                return new TelegramBotClient(options, httpClient);
            });

        services.AddHttpClient<LlmApi>(client =>
        {
            client.BaseAddress = new Uri(config["Llm:Provider"]!);
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", config["Llm:ApiKey"]);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });
    }

    private static void AddConfiguredClient<TClient>(this IServiceCollection services, 
        Action<HttpClient> configureClient) 
        where TClient : class
    {
        services.AddHttpClient<TClient>(configureClient)
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                UseDefaultCredentials = true,
                Credentials = CredentialCache.DefaultCredentials
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));
    }
}
