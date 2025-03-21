namespace MovieBot.Infrastructure.Swagger;

public static class ApplicationBuiderExtensions
{
    public static void UseSwaggerWithOAuth(this IApplicationBuilder builder)
    {
        builder.UseSwagger();
        builder.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieBot v1");
            o.RoutePrefix = "swagger";
        });
    }
}
