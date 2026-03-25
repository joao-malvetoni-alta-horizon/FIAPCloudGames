using FCG.Application.Games.Interfaces;
using FCG.Application.Games.UseCases;
using FCG.Domain.Games.Interfaces;
using FCG.Infrastructure.Persistence.Context;
using FCG.Infrastructure.Persistence.Repositories;
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

        return services;
    }
}
