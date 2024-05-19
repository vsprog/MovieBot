using Microsoft.AspNetCore.Mvc;
using MovieBot.Filters;
using MovieBot.Services;
using Telegram.Bot.Types;

namespace MovieBot.Controllers;

public class TelegramController : Controller
{
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
