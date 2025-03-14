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
    private readonly List<LlmMessage> _previousMessages = new();
    
    public LlmApi(HttpClient client, IOptions<LlmConfiguration> llmOptions)
    {
        _client = client;
        _llmConfig = llmOptions.Value;
    }

    public async Task<IEnumerable<string>> GetAnswer(string message, CancellationToken cancellationToken)
    {
        _previousMessages.Add(new LlmMessage(LlmRole.User, message));
        
        var response = await _client.PostAsJsonAsync(string.Empty, new LlmRequest
        {
            Model = _llmConfig.Model,
            Messages = _previousMessages.ToArray()
        }, cancellationToken);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return new[]{ Constants.DefaultLlmAnswer };
        }
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonConvert.DeserializeObject<LlmResponse>(content);

        var answers =  data!.Choices.Select(c => c.Message.Content).ToList();

        _previousMessages.AddRange(answers.Select(a => new LlmMessage(LlmRole.Assistant, a)));
        
        return answers;
    }
}
