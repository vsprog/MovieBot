using Microsoft.AspNetCore.Mvc;
using MovieBot.Services;

namespace MovieBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly YohohoService _yohohoService;
        private readonly LabService _labService;

        public HomeController(YohohoService yohohoService, LabService labService)
        {
            _yohohoService = yohohoService;
            _labService = labService;
        }

        [Route("/yohoho/{query}")]
        public async Task<IActionResult> GetMovieYohoho(string query)
        {
            var movies = await _yohohoService.GetMovies(query);
            
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            
            return Ok(movies);
        }
        
        [Route("/lab/{query}")]
        public async Task<IActionResult> GetMovieLab(string query)
        {
            var movies = await _labService.GetMovies(query);
            
            if (movies.Count == 0)
            {
                return NotFound();
            }
            
            return Ok(movies);
        }
    }
}
