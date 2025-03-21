using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using MovieBot.ExternalSources.Llm.Models;
using MovieBot.Infrastructure.Configurations;
using MovieBot.Services;

namespace MovieBot.ExternalSources.Llm;

public class LlmApi
{
    private readonly HttpClient _client;
    private readonly LlmConfiguration _llmConfig;
    private readonly JsonSerializerOptions _options;
    private readonly MessageHistoryService _historyService;
    
    public LlmApi(HttpClient client, IOptions<LlmConfiguration> llmOptions, MessageHistoryService historyService)
    {
        _client = client;
        _historyService = historyService;
        _llmConfig = llmOptions.Value;
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    public async Task<IEnumerable<string>> GetAnswer(string chatId, string message, CancellationToken cancellationToken, string prompts = "")
    {
        if (!string.IsNullOrEmpty(prompts))
        {
            _historyService.AddMessages(chatId, new List<LlmMessage>{new(LlmRole.System, prompts)});
        }
        
        _historyService.AddMessages(chatId, new List<LlmMessage>{new(LlmRole.User, message)});
        
        var response = await _client.PostAsJsonAsync(string.Empty, new LlmRequest
        {
            Model = _llmConfig.Model,
            Temperature = 1,
            Messages = _historyService.GetHistory(chatId)
        }, _options, cancellationToken);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new InvalidOperationException(response.StatusCode.ToString() + response.Content.ReadAsStringAsync(cancellationToken));
        }
        
        var content = await response.Content.ReadFromJsonAsync<LlmResponse>(_options, cancellationToken);
        var answers =  content!.Choices.Select(c => c.Message.Content).ToList();
        
        _historyService.AddMessages(chatId, answers.Select(a => new LlmMessage(LlmRole.Assistant, a)).ToList());
        
        return answers;
    }
}
