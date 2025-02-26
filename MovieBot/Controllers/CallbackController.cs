using Microsoft.AspNetCore.Mvc;
using MovieBot.Services;
using VkNet.Model;

namespace MovieBot.Controllers;

public class CallbackController : Controller
{
    [HttpPost]
    public async Task<IActionResult> Respond(
        [FromBody] GroupUpdate updates, 
        [FromServices] VkMessageHandlerService handlerService,
        CancellationToken cancellationToken)
    {
        var result = await handlerService.HandleUpdateAsync(updates, cancellationToken);
        return Ok(result);
    }
}

