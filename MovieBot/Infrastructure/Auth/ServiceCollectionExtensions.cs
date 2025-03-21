using Microsoft.AspNetCore.Authentication;

namespace MovieBot.Infrastructure.Auth;

public static class ServiceCollectionExtensions
{
    public static void AddGitHubAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication()
            .AddGitHub(o =>
            {
                o.ClientId = config["GithubAuth:ClientId"]!;
                o.ClientSecret = config["GithubAuth:ClientSecret"]!;
                o.CallbackPath = config["GithubAuth:CallbackPath"]!;
            })
            .AddIdentityServerJwt();
    }
}
