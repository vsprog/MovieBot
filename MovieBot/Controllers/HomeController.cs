using Microsoft.AspNetCore.Mvc;
using MovieBot.Services;

namespace MovieBot.Controllers
{
    public class HomeController : Controller
    {
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
    }
}
