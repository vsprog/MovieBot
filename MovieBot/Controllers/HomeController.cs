using Microsoft.AspNetCore.Mvc;
using MovieBot.ExternalSources.Llm;
using MovieBot.Services;

namespace MovieBot.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/yohoho/{query}")]
        public async Task<IActionResult> GetMovieYohoho(string query, 
            [FromServices] YohohoService yohohoService,
            CancellationToken cancellationToken)
        {
            var movies = await yohohoService.GetMovies(query, cancellationToken);
            
            return movies.Count == 0 
                ? NotFound() 
                : Ok(movies);
        }
        
        [HttpGet]
        [Route("/lab/{query}")]
        public async Task<IActionResult> GetMovieLab(string query, 
            [FromServices] LabService labService,
            CancellationToken cancellationToken)
        {
            var movies = await labService.GetMovies(query, cancellationToken);
            
            return movies.Count == 0 
                ? NotFound() 
                : Ok(movies);
        }
        
        [HttpGet]
        [Route("/llm/{query}")]
        public async Task<IActionResult> GetLlmAnswer(string query, 
            [FromServices] LlmApi llmApi,
            CancellationToken cancellationToken)
        {
            return Ok(await llmApi.GetAnswer(Environment.UserName, query, cancellationToken));
        }

        [HttpGet]
        [Route("/show/{link}/{title}")]
        public IActionResult Show(string link, string title)
        {
            return View((Uri.UnescapeDataString(link), title));
        }
    }
}
