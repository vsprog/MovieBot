using System.Net;
using Microsoft.Extensions.Options;
using MovieBot.ExternalSources.Llm.Models;
using MovieBot.Infractructure;

namespace MovieBot.ExternalSources.Llm;

public class LlmApi
{
    private readonly HttpClient _client;
    private readonly LlmConfiguration _llmConfig;
    
    public LlmApi(HttpClient client, IOptions<LlmConfiguration> llmOptions)
    {
        _client = client;
        _llmConfig = llmOptions.Value;
    }

    public async Task<string> GetAnswer(string message, CancellationToken cancellationToken)
    {
        var response = await _client.PostAsJsonAsync(string.Empty, new LlmRequest
        {
            Model = _llmConfig.Model,
            Messages = new[] { new LlmMessage("user", message) }
        }, cancellationToken);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return Constants.DefaultLlmAnswer;
        }
        
        var content = await response.Content.ReadFromJsonAsync<LlmResponse>(cancellationToken: cancellationToken);
        return content!.Choices[0].LlmMessage.Content;
    }
}
