using MovieBot.Handlers;
using MovieBot.Infrastructure.Auth;
using MovieBot.Infrastructure.Configurations;
using MovieBot.Infrastructure.HttpClients;
using MovieBot.Infrastructure.Swagger;
using MovieBot.Services;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfiguration(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGitHubAuth(builder.Configuration);
builder.Services.AddSwagger();
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddScoped<YohohoService>();
builder.Services.AddScoped<LabService>();
builder.Services.AddScoped<VkMessageHandlerService>();
builder.Services.AddScoped<TgMessageHandlerService>();
builder.Services.AddSingleton<IVkApi>(_ =>
{
    var api = new VkApi();
    api.Authorize(new ApiAuthParams { AccessToken = builder.Configuration["Vk:AccessToken"] });
    return api;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddSingleton<MessageHistoryService>();

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.ConfigureSwagger(builder.Configuration["GithubAuth:ClientId"]!);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
