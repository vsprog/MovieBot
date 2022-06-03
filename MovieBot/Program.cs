using System.Net;
using System.Net.Http.Headers;

using MovieBot.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHttpClient<MovieFinder>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ContentSources:Searcher"]);
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler()
        {
            UseDefaultCredentials = true,
            Credentials = CredentialCache.DefaultCredentials
        };
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));
builder.Services
    .AddHttpClient<MovieMaker>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ContentSources:Yohoho"]);
        client.DefaultRequestHeaders.Add("referer", builder.Configuration["ContentSources:YohoReferer"]);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler()
        {
            UseDefaultCredentials = true,
            Credentials = CredentialCache.DefaultCredentials
        };
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));
builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
