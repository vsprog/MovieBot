using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MovieBot.Infrastructure.Configurations;

namespace MovieBot.Infrastructure.Auth;

public class ApiKeyAuthenticationSchemeHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    private readonly string _apikeyname = "Apikey";
    
    public ApiKeyAuthenticationSchemeHandler(IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue(_apikeyname, out var extractedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Api Key was not provided"));
        }
        
        if (!extractedApiKey.Equals(Options.ApiKey)) 
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Apikey"));
        }
        
        var claims = new[] { new Claim(ClaimTypes.Name, "VALID USER") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
