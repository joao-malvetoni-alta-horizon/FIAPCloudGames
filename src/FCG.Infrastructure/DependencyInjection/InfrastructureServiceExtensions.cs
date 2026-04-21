using System.Security.Claims;
using System.Text;
using FCG.Application.Auth.Interfaces;
using FCG.Application.Auth.UseCases;
using FCG.Application.Games.Interfaces;
using FCG.Application.Games.UseCases;
using FCG.Application.Users.Interfaces;
using FCG.Application.Users.UseCases;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Shared;
using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Persistence.Context;
using FCG.Domain.Users.Services;
using FCG.Infrastructure.Persistence.Repositories;
using FCG.Infrastructure.Security;
using FCG.Domain.Users.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FCG.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGamePromotionRepository, GamePromotionRepository>();

        services.AddScoped<ICreateGameUseCase, CreateGameUseCase>();
        services.AddScoped<IGetGameUseCase, GetGameUseCase>();
        services.AddScoped<IListGamesUseCase, ListGamesUseCase>();
        services.AddScoped<IUpdateGameUseCase, UpdateGameUseCase>();
        services.AddScoped<IDeleteGameUseCase, DeleteGameUseCase>();
        services.AddScoped<ICreatePromotionUseCase, CreatePromotionUseCase>();
        services.AddScoped<IUpdatePromotionUseCase, UpdatePromotionUseCase>();
        services.AddScoped<IDeletePromotionUseCase, DeletePromotionUseCase>();
        services.AddScoped<IGetPromotionUseCase, GetPromotionUseCase>();
        services.AddScoped<IListPromotionsByGameUseCase, ListPromotionsByGameUseCase>();
        services.AddScoped<IPurchaseOwnedGameUseCase, PurchaseOwnedGameUseCase>();
        services.AddScoped<IGetUserOwnedGamesUseCase, GetUserOwnedGamesUseCase>();
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<IAdminCreateUserUseCase, AdminCreateUserUseCase>();
        services.AddScoped<IListUsersUseCase, ListUsersUseCase>();
        services.AddScoped<IGetUserDetailUseCase, GetUserDetailUseCase>();
        services.AddScoped<IAdminUpdateUserUseCase, AdminUpdateUserUseCase>();
        services.AddScoped<IAdminDeleteUserUseCase, AdminDeleteUserUseCase>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserOwnedGameRepository, UserOwnedGameRepository>();
        services.AddScoped<UnitOfWork>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<UnitOfWork>());
        services.AddScoped<IUserUnitOfWork>(provider => provider.GetRequiredService<UnitOfWork>());
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<DatabaseSeeder>();

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        services.AddSingleton(jwtSettings);
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ILoginUseCase, LoginUseCase>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole(RoleType.Administrator.ToRoleName()));
        });

        return services;
    }
}
