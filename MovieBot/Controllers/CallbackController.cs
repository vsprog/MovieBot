using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MovieBot.Services;

namespace MovieBot.Controllers;

public class CallbackController : Controller
{
    private readonly VkMessageHandlerService _handlerService;
    
    public CallbackController(VkMessageHandlerService handlerService)
    {
        _handlerService = handlerService;
    }

    [HttpPost]
    public async Task<IActionResult> Respond([FromBody] JsonElement updates, CancellationToken cancellationToken)
    {
        var result = await _handlerService.HandleUpdateAsync(updates, cancellationToken);
        return Ok(result);
    }
}

