using Microsoft.AspNetCore.Mvc;

using MovieBot.Models;
using MovieBot.Services;

namespace MovieBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieFinder finder;
        private readonly MovieMaker movies;

        public HomeController(MovieFinder finder, MovieMaker movies)
        {
            this.finder = finder;
            this.movies = movies;
        }

        [Route("/find/{query}")]
        public async Task<IActionResult> Information(string query)
        {
            var movieId = await finder.Search(query);

            if (movieId == null)
            {
                return NotFound();
            }

            var frames = await movies.GetFrames(movieId);
            var result = frames.ToArray();

            if (result.Length == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
