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
        _previousMessages.Add(new LlmMessage("user", message));
        
        var json = JsonConvert.SerializeObject(new LlmRequest
        {
            Model = _llmConfig.Model,
            Messages = _previousMessages
        });
        
        var response = await _client.PostAsync(string.Empty, new StringContent(json), cancellationToken);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            //Constants.DefaultLlmAnswer
            return new[]{ json, response.StatusCode.ToString() };
        }
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonConvert.DeserializeObject<LlmResponse>(content);

        var answers =  data!.Choices.Select(c => c.Message.Content).ToList();

        _previousMessages.AddRange(answers.Select(a => new LlmMessage("assistant", a)));
        
        return answers;
    }
}
