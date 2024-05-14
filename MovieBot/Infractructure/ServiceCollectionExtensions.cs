using System.Net;
using System.Net.Http.Headers;
using MovieBot.ExternalSources.Kinopoisk;
using MovieBot.ExternalSources.MovieLab;
using MovieBot.ExternalSources.Yohoho;

namespace MovieBot.Infractructure;

public static class ServiceCollectionExtensions
{
    public static void AddHttpClients(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddConfiguredClient<KinopoiskClient>(client =>
        {
            client.BaseAddress = new Uri(config["ContentSources:Kinopoisk"]);
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
        });

        services.AddConfiguredClient<YohohoClient>(client =>
        {
            client.BaseAddress = new Uri(config["ContentSources:Yohoho"]);
            client.DefaultRequestHeaders.Add("referer", config["ContentSources:YohoReferer"]);
        });
        
        services.AddConfiguredClient<MovieLabClient>(client =>
        {
            client.BaseAddress = new Uri(config["ContentSources:Lab"]);
            client.DefaultRequestHeaders.Add("Server", "cloudflare");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });
    }

    private static void AddConfiguredClient<TClient>(this IServiceCollection services, 
        Action<HttpClient> configureClient) 
        where TClient : class
    {
        services.AddHttpClient<TClient>(configureClient)
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                UseDefaultCredentials = true,
                Credentials = CredentialCache.DefaultCredentials
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));
    }
}
