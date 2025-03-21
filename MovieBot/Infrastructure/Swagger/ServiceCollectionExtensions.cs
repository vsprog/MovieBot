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
        });
    }
}
