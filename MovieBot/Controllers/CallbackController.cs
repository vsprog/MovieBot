using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MovieBot.Infractructure;
using MovieBot.Services;
using Newtonsoft.Json;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace MovieBot.Controllers
{
    public class CallbackController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IVkApi _vkApi;
        private readonly YohohoService _yohohoService;
        private readonly LabService _labService;

        public CallbackController(
            IConfiguration configuration, 
            IVkApi vkApi,
            YohohoService yohohoService, 
            LabService labService)
        {
            _configuration = configuration;
            _vkApi = vkApi;
            _yohohoService = yohohoService;
            _labService = labService;
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
            var message = new MessagesSendParams
            {
                RandomId = new DateTime().Millisecond,
                PeerId = income?.PeerId,
                Message = string.Empty
            };

            switch (income)
            {
                case null:
                case { Text: var text } when !text.StartsWith("найди ", StringComparison.CurrentCultureIgnoreCase):
                    message.Message = "Чтобы воспользоваться поиском, напишите: \"найди название_фильма\" ";
                    break;
                default:
                    var title = income.Text[(income.Text.IndexOf(' ') + 1)..];
                    var movies = await _labService.GetMovies(title);

                    message.Message = movies.Count == 0 
                        ? Constants.Answers[rndInd]
                        : string.Concat(movies.Select(m => $"{m.Title} \n {m.Url} \n"));
                    message.Attachments = await UploadPosters(income.UserId, movies.Select(m => m.PosterLink!));
                    
                    break;
            }

            Send(message);
            return Ok("ok");
        }

        private void Send(MessagesSendParams message)
        {
            if (!Request.Headers.ContainsKey("X-Retry-Counter"))
            {
                _vkApi.Messages.Send(message);
            }
        }

        [Obsolete("Obsolete")]
        private async Task<IEnumerable<Photo>> UploadPosters(long? userId, IEnumerable<string> links)
        {
            var uploadServer = _vkApi.Photo.GetMessagesUploadServer(userId);
            using var wc = new WebClient();

            return await Task.WhenAll(links.Select(async link =>
            {
                var image = await wc.DownloadDataTaskAsync(link);
                var uploadResponse = Encoding.ASCII.GetString(await wc.UploadDataTaskAsync(uploadServer.UploadUrl, image));
                var data = JsonConvert.DeserializeObject<JsonElement>(uploadResponse);
                return _vkApi.Photo.SaveMessagesPhoto(data.GetProperty("upload_url").ToString()).FirstOrDefault();
            }));
        }
    }
}
