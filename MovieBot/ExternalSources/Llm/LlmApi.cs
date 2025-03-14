using System.Net;
using Microsoft.Extensions.Options;
using MovieBot.ExternalSources.Llm.Models;
using MovieBot.Infractructure;
using Newtonsoft.Json;

namespace MovieBot.ExternalSources.Llm;

public class LlmApi
{
    private readonly HttpClient _client;
    private readonly LlmConfiguration _llmConfig;
    private readonly bool _isStream;
    
    public LlmApi(HttpClient client, IOptions<LlmConfiguration> llmOptions)
    {
        _client = client;
        _llmConfig = llmOptions.Value;
        _isStream = true;
    }

    public async Task<IEnumerable<string>> GetAnswer(string message, CancellationToken cancellationToken)
    {
        var response = await _client.PostAsJsonAsync(string.Empty, new LlmRequest
        {
            Model = _llmConfig.Model,
            Temperature = 1,
            Top = 1,
            Stream = _isStream,
            Messages = new[] { new LlmMessage("user", message) }
        }, cancellationToken);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return new[]{ Constants.DefaultLlmAnswer };
        }
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return new[]{content};
        // var data = JsonConvert.DeserializeObject<LlmResponse>(content);
        //
        // return data!.Choices.Select(c => _isStream 
        //     ? c.Delta?.Content ?? string.Empty
        //     : c.Message.Content);
    }
}
