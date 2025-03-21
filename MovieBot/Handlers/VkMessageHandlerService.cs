using MovieBot.ExternalSources;
using MovieBot.Helpers;
using MovieBot.Services;
using VkNet.Abstractions;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace MovieBot.Handlers;

public class VkMessageHandlerService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IVkApi _vkApi;
    private readonly YohohoService _yohohoService;
    private readonly LabService _labService;
    private readonly FileLoader _fileLoader;
    
    public VkMessageHandlerService(IConfiguration configuration, 
        IVkApi vkApi, 
        YohohoService yohohoService, 
        LabService labService, 
        FileLoader fileLoader, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _vkApi = vkApi;
        _yohohoService = yohohoService;
        _labService = labService;
        _fileLoader = fileLoader;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> HandleUpdateAsync(GroupUpdate update, CancellationToken cancellationToken)
    {
        return update.Type.Value switch
        {
            GroupUpdateType.Confirmation => _configuration["Vk:Confirmation"],
            GroupUpdateType.MessageNew => await HandleMessageAsync(((MessageNew)update.Instance).Message, cancellationToken),
            _ => throw new ArgumentOutOfRangeException("Invalid type property")
        };
    }

    private async Task<string> HandleMessageAsync(Message? incoming, CancellationToken cancellationToken)
    {
        var random = new Random();
        var rndInd = random.Next(0, Constants.Answers.Length);
        var messages = new List<MessagesSendParams>
        {
            new()
            {
                RandomId = new DateTime().Millisecond,
                PeerId = incoming?.PeerId,
                Message = string.Empty
            }
        };

        switch (incoming)
        {
            case null:
            case { Text: var text } when !text.StartsWith("найди ", StringComparison.CurrentCultureIgnoreCase):
                messages[0].Message = "Чтобы воспользоваться поиском, напишите: \"найди название_фильма\" ";
                break;
            default:
                var title = incoming.Text[(incoming.Text.IndexOf(' ') + 1)..];
                var movies = await _labService.GetMovies(title, cancellationToken);

                if (movies.Count == 0)
                {
                    messages[0].Message = Constants.Answers[rndInd];
                    break;
                }

                messages = (await Task.WhenAll(movies.Select(async m =>
                    new MessagesSendParams
                    {
                        RandomId = new DateTime().Millisecond, PeerId = incoming?.PeerId,
                        Message = $"{m.Title} \n {m.Url} \n",
                        Attachments = await UploadPoster(incoming?.UserId, m.PosterLink, cancellationToken)
                    })
                )).ToList();
                break;
        }
        
        messages.ForEach(Send);
        return string.Empty;
    }
    
    private void Send(MessagesSendParams message)
    {
        if (!_httpContextAccessor.HttpContext!.Request.Headers.ContainsKey("X-Retry-Counter"))
        {
            _vkApi.Messages.Send(message);
        }
    }
    
    private async Task<IEnumerable<Photo>> UploadPoster(long? userId, string? link, CancellationToken cancellationToken)
    {
        if (link is null)
        {
            return Enumerable.Empty<Photo>();
        }
        
        var uploadServer = _vkApi.Photo.GetMessagesUploadServer(userId);
        var response = await _fileLoader.UploadFile(uploadServer.UploadUrl, link, "jpg", cancellationToken);
        return _vkApi.Photo.SaveMessagesPhoto(response);
    }
}
