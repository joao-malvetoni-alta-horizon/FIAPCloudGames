using System.Text.Json.Serialization;
using FCG.API.Configuration;
using FCG.API.Endpoints;
using FCG.API.Middlewares;
using FCG.Infrastructure.DependencyInjection;
using FCG.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwaggerConfig();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.UseSwaggerConfig();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapGameEndpoints();
app.MapUserEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var retries = 10;
    for (var i = 0; i < retries; i++)
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (Exception ex) when (i < retries - 1)
        {
            app.Logger.LogWarning(ex, "Database not ready yet, retrying in 3s... ({Attempt}/{MaxRetries})", i + 1,
                retries);
            Thread.Sleep(3000);
        }
}

app.Run();