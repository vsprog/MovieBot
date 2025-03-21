namespace MovieBot.Infrastructure.Swagger;

public static class ApplicationBuiderExtensions
{
    public static void ConfigureSwagger(this IApplicationBuilder builder)
    {
        builder.UseSwagger();
        builder.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieBot v1");
            options.RoutePrefix = "swagger";
        });
    }
}
