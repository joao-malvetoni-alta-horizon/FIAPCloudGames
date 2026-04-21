using Microsoft.OpenApi;

namespace FCG.API.Configuration;

public static class SwaggerConfig
{
    private const string BearerScheme = "Bearer";

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(BearerScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the JWT token using the format: Bearer {token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = BearerScheme,
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference(BearerScheme),
                    new List<string>()
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG API v1");
            options.RoutePrefix = "swagger";
        });

        return app;
    }
}