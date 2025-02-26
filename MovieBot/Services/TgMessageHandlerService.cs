using MovieBot.ExternalSources.Llm;
using MovieBot.Infractructure;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MovieBot.Services;

public class TgMessageHandlerService
{
    private readonly ITelegramBotClient _botClient;
    private readonly LabService _labService;
    private readonly LlmApi _llmApi;

    public TgMessageHandlerService(ITelegramBotClient botClient, LabService labService, LlmApi llmApi)
    {
        _botClient = botClient;
        _labService = labService;
        _llmApi = llmApi;
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
            "/start" => SendTextMessage(message, "Для просмотра фильма напишите: \"/find название_фильма\".\nДля общения напишите запрос в произвольной форме", cancellationToken),
            "/find" => SendMoviesResult(message, cancellationToken), 
            _ => _llmApi.GetAnswer(message.Text, cancellationToken)
        };
        
        await action;
    }

    private async Task SendTextMessage(Message incoming, string text,
        CancellationToken cancellationToken)
    { 
        await _botClient.SendTextMessageAsync(
            chatId: incoming.Chat.Id,
            text: text,
            cancellationToken: cancellationToken);
    }

    private async Task SendMoviesResult(Message incoming, CancellationToken cancellationToken)
    {
        var title = incoming.Text![(incoming.Text.IndexOf(' ') + 1)..];
        var movies = await _labService.GetMovies(title, cancellationToken);

        if (movies.Count == 0)
        {
            var rndInd = new Random().Next(0, Constants.Answers.Length);
            await SendTextMessage(incoming, Constants.Answers[rndInd], cancellationToken);
            return;
        }

        foreach (var movie in movies)
        {
            await _botClient.SendPhotoAsync(
                chatId: incoming.Chat.Id,
                photo: InputFile.FromUri(movie.PosterLink ?? string.Empty),
                caption: $"Ссылка для просмотра: <a href=\"{movie.Url}\">{movie.Title}</a>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }
    }
}
