using MovieBot.ExternalSources.Llm;
using MovieBot.Helpers;
using MovieBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MovieBot.Handlers;

public class TgMessageHandlerService
{
    private readonly ITelegramBotClient _botClient;
    private readonly LabService _labService;
    private readonly LlmApi _llmApi;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TgMessageHandlerService(ITelegramBotClient botClient, LabService labService, 
        LlmApi llmApi, IHttpContextAccessor httpContextAccessor)
    {
        _botClient = botClient;
        _labService = labService;
        _llmApi = llmApi;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // UpdateType.Unknown:
        // UpdateType.ChannelPost:
        // UpdateType.EditedChannelPost:
        // UpdateType.ShippingQuery:
        // UpdateType.PreCheckoutQuery:
        // UpdateType.Poll:
        var handler = update switch
        {
            { Message: { } message } => OnMessageReceived(message, cancellationToken),
            { EditedMessage: { } message } => OnMessageReceived(message, cancellationToken),
            _  => Task.CompletedTask
        };

        await handler;
    }

    private async Task OnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(message.Text))
        {
            return;
        }
        
        var action = message.Text.Split(' ')[0] switch
        {
            "/start" => SendTextMessage(message.Chat.Id, "Для просмотра фильма напишите: \"/find название_фильма\".\nДля общения напишите запрос в произвольной форме", cancellationToken),
            "/find" => SendMoviesResult(message, cancellationToken), 
            _ => SendLlmMessage(message, cancellationToken)
            
            //TODO: add prompts
            // /einstein => SendLlmMessage(message, "prompts", cancellationToken)
            // /drBrown_ => SendLlmMessage(message, "prompts", cancellationToken)
        };
        
        await action;
    }

    private async Task SendLlmMessage(Message incoming, CancellationToken cancellationToken)
    {
        IEnumerable<string> answers;
        
        try
        {
            answers = await _llmApi.GetAnswer(incoming.Chat.Id.ToString(), incoming.Text!, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            answers = [Constants.DefaultLlmAnswer];
        }

        foreach (var answer in answers)
        {
            await SendTextMessage(incoming.Chat.Id, answer, cancellationToken);
        }
    }

    private async Task SendMoviesResult(Message incoming, CancellationToken cancellationToken)
    {
        var titleStrings = incoming.Text!.Split(' ', 2);
        
        if (titleStrings.Length < 2)
        {
            await SendTextMessage(incoming.Chat.Id, "пропущено \"название_фильма\"", cancellationToken);
            return;
        }
        
        var movies = await _labService.GetMovies(titleStrings[1], cancellationToken);
        
        if (movies.Count == 0)
        {
            var rndInd = new Random().Next(0, Constants.Answers.Length);
            await SendTextMessage(incoming.Chat.Id, Constants.Answers[rndInd], cancellationToken);
            return;
        }
        
        foreach (var movie in movies)
        {
            await _botClient.SendPhotoAsync(
                chatId: incoming.Chat.Id,
                photo: InputFile.FromUri(movie.PosterLink ?? string.Empty),
                caption: $"Ссылка для просмотра: <a href=" +
                         $"\"{_httpContextAccessor.HttpContext?.Request.Scheme}://" +
                         $"{_httpContextAccessor.HttpContext?.Request.Host}/show/?link=" +
                         $"{Uri.EscapeDataString(movie.Url)}&title={movie.Title}\">{movie.Title}</a>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }
    }
    
    private async Task SendTextMessage(long chatId, string text, CancellationToken cancellationToken)
    { 
        await _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            cancellationToken: cancellationToken);
    }
}
