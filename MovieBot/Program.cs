using System.Net;
using System.Net.Http.Headers;
using MovieBot.ExternalSources.Kinopoisk;
using MovieBot.ExternalSources.MovieLab;
using MovieBot.ExternalSources.Yohoho;
using MovieBot.Services;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddSingleton<IVkApi>(_ => 
    {
        var api = new VkApi();
        api.Authorize(new ApiAuthParams { AccessToken = builder.Configuration["Config:AccessToken"] });
        return api;
    })
    .AddHttpClient<KinopoiskClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ContentSources:Kinopoisk"]);
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
    {
        UseDefaultCredentials = true,
        Credentials = CredentialCache.DefaultCredentials
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services
    .AddHttpClient<YohohoClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ContentSources:Yohoho"]);
        client.DefaultRequestHeaders.Add("referer", builder.Configuration["ContentSources:YohoReferer"]);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
    {
        UseDefaultCredentials = true,
        Credentials = CredentialCache.DefaultCredentials
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services
    .AddHttpClient<MovieLabClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ContentSources:Lab"]);
        client.DefaultRequestHeaders.Add("Server", "cloudflare");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
    {
        UseDefaultCredentials = true,
        Credentials = CredentialCache.DefaultCredentials
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services.AddSingleton<YohohoService, YohohoService>();
builder.Services.AddSingleton<LabService, LabService>();

builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
