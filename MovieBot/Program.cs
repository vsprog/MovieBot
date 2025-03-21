using MovieBot.Handlers;
using MovieBot.Infrastructure.Configurations;
using MovieBot.Infrastructure.HttpClients;
using MovieBot.Infrastructure.Swagger;
using MovieBot.Services;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

var builder = WebApplication.CreateBuilder(args);

var botConfigurationSection = builder.Configuration.GetSection(TelegramConfiguration.Configuration);
builder.Services.Configure<TelegramConfiguration>(botConfigurationSection);
var llmConfiguration = builder.Configuration.GetSection(LlmConfiguration.Configuration);
builder.Services.Configure<LlmConfiguration>(llmConfiguration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddScoped<YohohoService>();
builder.Services.AddScoped<LabService>();
builder.Services.AddScoped<VkMessageHandlerService>();
builder.Services.AddScoped<TgMessageHandlerService>();
builder.Services.AddSingleton<IVkApi>(_ =>
{
    var api = new VkApi();
    api.Authorize(new ApiAuthParams { AccessToken = builder.Configuration["Config:AccessToken"] });
    return api;
});

builder.Services.AddHostedService<ConfigureWebhook>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddSingleton<MessageHistoryService>();

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.ConfigureSwagger();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
