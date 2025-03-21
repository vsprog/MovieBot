using System.Reflection;
using Microsoft.OpenApi.Models;

namespace MovieBot.Infrastructure.Swagger;

public static class ServiceCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "MovieBot api",
            });
            options.IncludeXmlComments(Assembly.GetExecutingAssembly());
            options.CustomSchemaIds(type => type.FullName);
            
            options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(configuration["GithubAuth:Url"]!)
                    }
                }
            });
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "OAuth2"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
