using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MovieBot.Infrastructure.Auth;

public static class ServiceCollectionExtensions
{
    public static void AddGitHubAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
    }
}
