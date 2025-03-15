using System.Collections.Concurrent;
using MovieBot.ExternalSources.Llm.Models;

namespace MovieBot.Services;

public class MessageHistoryService
{
    private readonly int _capacity = 20;
    private readonly ConcurrentDictionary<string, List<LlmMessage>> _messageHistory = new();

    public void AddMessages(string chatId, List<LlmMessage> messages)
    {
       _messageHistory.AddOrUpdate(chatId, messages, 
           (_, oldList) =>
            {
                oldList.AddRange(messages);
                if (oldList.Count > _capacity)
                {
                    oldList.RemoveRange(0, oldList.Count - _capacity);
                }
                return oldList;
            });
    }
    
    public List<LlmMessage> GetHistory(string chatId)
    {
        return _messageHistory.TryGetValue(chatId, out var messages)
            ? messages
            : new List<LlmMessage>();
    }
}
