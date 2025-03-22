using Microsoft.AspNetCore.Authentication;

namespace MovieBot.Infrastructure.Configurations;

public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey {get; set;}
}
