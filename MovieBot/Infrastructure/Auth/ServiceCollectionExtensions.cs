namespace MovieBot.Infrastructure.Auth;

public static class ServiceCollectionExtensions
{
    public static void AddGitHubAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthorization();
        services.AddAuthentication("Bearer")
            .AddGitHub(o =>
            {
                o.ClientId = config["GithubAuth:ClientId"]!;
                o.ClientSecret = config["GithubAuth:ClientSecret"]!;
                o.CallbackPath = config["GithubAuth:CallbackPath"]!;
                o.Scope.Add("read:user");
            });
    }
}
