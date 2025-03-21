using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using MovieBot.ExternalSources.Llm;
using MovieBot.Services;
using MovieBot.Services.Models;

namespace MovieBot.Controllers
{
    /// <summary>
    /// Тестовые методы
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Поиск фильмов в yohoho
        /// </summary>
        /// <param name="query">Запрос для поиска</param>
        /// <param name="yohohoService">Сервис поиска фильмов</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<FilmDto>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(500)]
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
        
        /// <summary>
        /// Поиск фильмов в movielab
        /// </summary>
        /// <param name="query">Запрос для поиска</param>
        /// <param name="labService">Сервис поиска фильмов</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<FilmDto>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(500)]
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
        
        /// <summary>
        /// Общение с llm
        /// </summary>
        /// <param name="query">Сообщение</param>
        /// <param name="llmApi">Сервис обработки сообщения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal error</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(500)]
        [Route("/llm/{query}")]
        public async Task<IActionResult> GetLlmAnswer(string query, 
            [FromServices] LlmApi llmApi,
            CancellationToken cancellationToken)
        {
            IEnumerable<string> answers;
        
            try
            {
                answers = await llmApi.GetAnswer(Environment.UserName, query, cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                answers = [e.Message];
            }
            
            return Ok(answers);
        }

        /// <summary>
        /// Страница с iframe
        /// </summary>
        /// <param name="link">Ссылка</param>
        /// <param name="title">Описание</param>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal error</response>
        [HttpGet]
        [ProducesResponseType(typeof(HtmlString), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(500)]
        [Route("/show")]
        public IActionResult Show([FromQuery]string link, [FromQuery]string title)
        {
            return View((Uri.UnescapeDataString(link), title));
        }
    }
}
