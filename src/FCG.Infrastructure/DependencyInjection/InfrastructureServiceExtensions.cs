using FCG.Application.Games.Interfaces;
using FCG.Application.Games.UseCases;
using FCG.Application.Users.Interfaces;
using FCG.Application.Users.UseCases;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Shared;
using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Persistence.Context;
using FCG.Infrastructure.Persistence.Repositories;
using FCG.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IGameRepository, GameRepository>();

        services.AddScoped<ICreateGameUseCase, CreateGameUseCase>();
        services.AddScoped<IGetGameUseCase, GetGameUseCase>();
        services.AddScoped<IListGamesUseCase, ListGamesUseCase>();
        services.AddScoped<IUpdateGameUseCase, UpdateGameUseCase>();
        services.AddScoped<IDeleteGameUseCase, DeleteGameUseCase>();
        services.AddScoped<IPurchaseOwnedGameUseCase, PurchaseOwnedGameUseCase>();
        services.AddScoped<IGetUserOwnedGamesUseCase, GetUserOwnedGamesUseCase>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserGameLibraryRepository, UserGameLibraryRepository>();
        services.AddScoped<UnitOfWork>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<UnitOfWork>());
        services.AddScoped<IUserUnitOfWork>(provider => provider.GetRequiredService<UnitOfWork>());
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}
