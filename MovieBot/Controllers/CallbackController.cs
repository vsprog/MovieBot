using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MovieBot.Services;

namespace MovieBot.Controllers;

public class CallbackController : Controller
{
    [HttpPost]
    public async Task<IActionResult> Respond(
        [FromBody] JsonElement updates, 
        [FromServices] VkMessageHandlerService handlerService,
        CancellationToken cancellationToken)
    {
        var result = await handlerService.HandleUpdateAsync(updates, cancellationToken);
        return Ok(result);
    }
}

