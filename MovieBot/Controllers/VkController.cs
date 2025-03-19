using Microsoft.AspNetCore.Mvc;
using MovieBot.Services;
using VkNet.Model;

namespace MovieBot.Controllers;

/// <summary>
/// Обработчик vk
/// </summary>
[Route("vk")]
public class VkController : Controller
{
    /// <summary>
    /// Обработка сообщений vk
    /// </summary>
    /// <param name="updates">Событие vk</param>
    /// <param name="handlerService">Сервис обработки запросов</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <response code="200">Success</response>
    /// <response code="400">Invalid request</response>
    /// <response code="500">Internal error</response>
    [HttpPost]
    [Route("respond")]
    public async Task<IActionResult> Respond(
        [FromBody] GroupUpdate updates, 
        [FromServices] VkMessageHandlerService handlerService,
        CancellationToken cancellationToken)
    {
        var result = await handlerService.HandleUpdateAsync(updates, cancellationToken);
        return Ok(result);
    }
}

