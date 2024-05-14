using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MovieBot.ExternalSources;
using MovieBot.Infractructure;
using MovieBot.Services;
using Newtonsoft.Json;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace MovieBot.Controllers;

public class CallbackController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IVkApi _vkApi;
    private readonly YohohoService _yohohoService;
    private readonly LabService _labService;
    private readonly FileLoader _fileLoader;

    public CallbackController(
        IConfiguration configuration, 
        IVkApi vkApi, YohohoService yohohoService, 
        LabService labService, FileLoader fileLoader)
    {
        _configuration = configuration;
        _vkApi = vkApi;
        _yohohoService = yohohoService;
        _labService = labService;
        _fileLoader = fileLoader;
    }

    [HttpPost]
    public async Task<IActionResult> Respond([FromBody] JsonElement updates)
    {
        var type = updates.GetProperty("type").GetString();
        
        return type switch
        {
            "confirmation" => Ok(_configuration["Config:Confirmation"]),
            "message_new" => await GetAnswer(updates),
            _ => throw new ArgumentOutOfRangeException("Invalid type property")
        };
    }

    private async Task<IActionResult> GetAnswer(JsonElement updates)
    {
        var msgObject = updates.GetProperty("object").GetProperty("message");
        var income = JsonConvert.DeserializeObject<Message>(msgObject.ToString());
        var random = new Random();
        var rndInd = random.Next(0, Constants.Answers.Length);
        var messages = new List<MessagesSendParams>
        {
            new()
            {
                RandomId = new DateTime().Millisecond,
                PeerId = income?.PeerId,
                Message = string.Empty
            }
        };

        switch (income)
        {
            case null:
            case { Text: var text } when !text.StartsWith("найди ", StringComparison.CurrentCultureIgnoreCase):
                messages[0].Message = "Чтобы воспользоваться поиском, напишите: \"найди название_фильма\" ";
                break;
            default:
                var title = income.Text[(income.Text.IndexOf(' ') + 1)..];
                var movies = await _labService.GetMovies(title);

                if (movies.Count == 0)
                {
                    messages[0].Message = Constants.Answers[rndInd];
                    break;
                }

                messages = (await Task.WhenAll(movies.Select(async m =>
                    new MessagesSendParams
                    {
                        RandomId = new DateTime().Millisecond, PeerId = income?.PeerId,
                        Message = $"{m.Title} \n {m.Url} \n",
                        Attachments = await UploadPoster(income?.UserId, m.PosterLink)
                    })
                )).ToList();
                break;
        }
        
        messages.ForEach(Send);
        return Ok("ok");
    }

    private void Send(MessagesSendParams message)
    {
        if (!Request.Headers.ContainsKey("X-Retry-Counter"))
        {
            _vkApi.Messages.Send(message);
        }
    }
    
    private async Task<IEnumerable<Photo>> UploadPoster(long? userId, string? link)
    {
        if (link is null)
        {
            return Enumerable.Empty<Photo>();
        }
        
        var uploadServer = _vkApi.Photo.GetMessagesUploadServer(userId);
        var response = await _fileLoader.UploadFile(uploadServer.UploadUrl, link, "jpg");
        return _vkApi.Photo.SaveMessagesPhoto(response);
    }
}

