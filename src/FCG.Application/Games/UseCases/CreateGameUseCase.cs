using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Application.Games.Mappers;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Interfaces;

namespace FCG.Application.Games.UseCases;

public class CreateGameUseCase(IGameRepository repository) : ICreateGameUseCase
{
    public async Task<GameResponse> ExecuteAsync(CreateGameRequest request, CancellationToken ct = default)
    {
        var game = new Game(
            request.Title,
            request.Description,
            request.Price,
            request.Genre,
            request.ReleaseDate);

        await repository.AddAsync(game, ct);

        return GameMapper.ToResponse(game);
    }
}
