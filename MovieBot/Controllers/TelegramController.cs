using Microsoft.AspNetCore.Mvc;
using MovieBot.Filters;
using MovieBot.Services;
using Telegram.Bot.Types;

namespace MovieBot.Controllers;

/// <summary>
/// Обработчик telegram
/// </summary>
[Route("telegram")]
public class TelegramController : Controller
{
    /// <summary>
    /// Обработка сообщений telegram
    /// </summary>
    /// <param name="update">Событие telegram</param>
    /// <param name="handlerService">Сервис обработки запросов</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <response code="200">Success</response>
    /// <response code="400">Invalid request</response>
    /// <response code="500">Internal error</response>
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Respond(
        [FromBody] Update update, 
        [FromServices] TgMessageHandlerService handlerService,
        CancellationToken cancellationToken)
    {
        await handlerService.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }
}
