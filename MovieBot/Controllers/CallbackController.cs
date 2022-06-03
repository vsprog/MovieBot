using Microsoft.AspNetCore.Mvc;

using MovieBot.Infractructure;
using MovieBot.Models;
using MovieBot.Services;

using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;

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
        public async Task<IActionResult> Callback([FromBody] Updates updates)
        {
            switch (updates.Type)
            {
                case "confirmation":
                    return Ok(configuration["Config:Confirmation"]);

                case "message_new":
                    Message msg = Message.FromJson(new VkResponse(updates.Object));
                    string txt = msg.Text;
                    Random random = new Random();
                    int rndInd = random.Next(0, Constants.Answers.Length);
                    var messageParams = new MessagesSendParams
                    {
                        RandomId = new DateTime().Millisecond,
                        PeerId = msg.PeerId.Value,
                        Message = string.Empty
                    };

                    if (txt.StartsWith("Найди "))
                    {
                        string queryString = txt.Substring(txt.IndexOf(' '));
                        var movieId = await finder.Search(queryString);

                        if (movieId == null)
                        {
                            messageParams.Message = Constants.Answers[rndInd];
                            vkApi.Messages.Send(messageParams);
                            return Ok("ok");
                        }

                        var frames = await movies.GetFrames(movieId);
                        var result = frames.ToArray();

                        if (result.Length == 0)
                        {
                            messageParams.Message = Constants.Answers[rndInd];
                            vkApi.Messages.Send(messageParams);
                            return Ok("ok");
                        }

                        foreach (Frame fr in result)
                        {
                            messageParams.Message = $"В переводе {fr.Translate} \n {fr.Url}";
                            vkApi.Messages.Send(messageParams);
                        }
                    }
                    break;
            }

            return Ok("ok");
        }
    }
}
