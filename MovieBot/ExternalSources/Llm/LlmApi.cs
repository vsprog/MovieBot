using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using MovieBot.ExternalSources.Llm.Models;
using MovieBot.Infractructure;

namespace MovieBot.ExternalSources.Llm;

public class LlmApi
{
    private readonly HttpClient _client;
    private readonly LlmConfiguration _llmConfig;
    private readonly List<LlmMessage> _previousMessages = new();
    private readonly JsonSerializerOptions _options;
    
    public LlmApi(HttpClient client, IOptions<LlmConfiguration> llmOptions)
    {
        _client = client;
        _llmConfig = llmOptions.Value;
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    public async Task<IEnumerable<string>> GetAnswer(string message, CancellationToken cancellationToken)
    {
        _previousMessages.Add(new LlmMessage(LlmRole.User, message));
       
        var json = JsonSerializer.Serialize(new LlmRequest
        {
            Model = _llmConfig.Model,
            Temperature = 1,
            Messages = _previousMessages
        }, _options);

        var response = await _client.PostAsync(string.Empty, new StringContent(json), cancellationToken);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return new[]{ Constants.DefaultLlmAnswer };
        }
        
        var content = await response.Content.ReadFromJsonAsync<LlmResponse>(cancellationToken: cancellationToken);
        var answers =  content!.Choices.Select(c => c.Message.Content).ToList();

        _previousMessages.AddRange(answers.Select(a => new LlmMessage(LlmRole.Assistant, a)));
        
        return answers;
    }
}
