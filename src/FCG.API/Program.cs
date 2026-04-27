using System.Text;
using System.Text.Json.Serialization;
using FCG.API.Configuration;
using FCG.API.Endpoints;
using FCG.API.Middlewares;
using FCG.Application.Configuration;
using FCG.Application.Users.Interfaces;
using FCG.Application.Users.UseCases;
using FCG.Infrastructure.DependencyInjection;
using FCG.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar JWT Options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddInfrastructure(builder.Configuration);

// Configurar autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions?.Issuer,
            ValidAudience = jwtOptions?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions?.Key ?? ""))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerConfig();

// Registrar serviços de autenticação
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ILoginUseCase, LoginUseCase>();
builder.Services.AddScoped<IRegisterUseCase, RegisterUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.UseSwaggerConfig();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapGameEndpoints();
app.MapAuthEndpoints();
app.MapUserGameEndpoints();

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