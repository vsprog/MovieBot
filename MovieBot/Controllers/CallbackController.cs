using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using MovieBot.Infractructure;
using MovieBot.Models;
using MovieBot.Services;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace MovieBot.Controllers
{
    public class CallbackController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IVkApi vkApi;
        private readonly MovieFinder finder;
        private readonly MovieMaker movies;

        public CallbackController(
            IConfiguration configuration, 
            IVkApi vkApi,
            MovieFinder finder, 
            MovieMaker movies)
        {
            this.configuration = configuration;
            this.vkApi = vkApi;
            this.finder = finder;
            this.movies = movies;
        }

        [HttpPost]
        public async Task<IActionResult> Respond([FromBody] JsonElement updates)
        {
            var type = updates.GetProperty("type").GetString();

            switch (type)
            {
                case "confirmation":
                    return Ok(configuration["Config:Confirmation"]);
                case "message_new":
                    var msgObject = updates.GetProperty("object").GetProperty("message");
                    var message = JsonConvert.DeserializeObject<Message>(msgObject.ToString());
                    Random random = new Random();
                    int rndInd = random.Next(0, Constants.Answers.Length);
                    var messageParams = new MessagesSendParams
                    {
                        RandomId = new DateTime().Millisecond,
                        PeerId = message.PeerId,
                        Message = string.Empty
                    };

                    if (message.Text.ToLower().StartsWith("найди "))
                    {
                        string queryString = message.Text.Substring(message.Text.IndexOf(' ')+1);
                        var movieId = await finder.Search(queryString);

                        if (movieId == null)
                        {
                            messageParams.Message = Constants.Answers[rndInd];
                            Send(messageParams);

                            return Ok("ok");
                        }

                        var frames = await movies.GetFrames(movieId);
                        var result = frames.ToArray();

                        if (result.Length == 0)
                        {
                            messageParams.Message = "Не найдено озвучек";
                            Send(messageParams);

                            return Ok("ok");
                        }

                        foreach (Frame fr in result)
                        {
                            messageParams.Message = $"В озвучке {fr.Translate} \n {fr.Url}";
                            Send(messageParams);
                        }
                    }
                    else 
                    {
                        messageParams.Message = "Чтобы воспользоваться поиском, напишите: \"найди название_фильма\" ";
                        Send(messageParams);
                    }

                    break;
            }

            return Ok("ok");
        }

        private void Send(MessagesSendParams message)
        {
            if (!Request.Headers.Keys.Contains("X-Retry-Counter"))
            {
                vkApi.Messages.Send(message);
            }
        }
    }
}
