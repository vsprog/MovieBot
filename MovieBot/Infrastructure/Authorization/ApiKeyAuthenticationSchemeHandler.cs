using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MovieBot.Infrastructure.Configurations;

namespace MovieBot.Infrastructure.Authorization;

public class ApiKeyAuthenticationSchemeHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    private readonly string _apikeyname = "Apikey";
    
    public ApiKeyAuthenticationSchemeHandler(
        IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder) : base(options, logger, encoder)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.Request.Headers.TryGetValue(_apikeyname, out var extractedApiKey) && 
            string.Equals(extractedApiKey, Options.ApiKey, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateSuccess(Roles.Admin));
        }
        
        if (Context.Request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var telegramTokenHeader) &&
            string.Equals(telegramTokenHeader, Options.TelegramSecretToken, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateSuccess(Roles.TelegramBot));
        }
        
        return Task.FromResult(AuthenticateResult.Fail("Invalid authentication headers"));
    }
    
    private AuthenticateResult AuthenticateSuccess(string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, role),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
