using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Application.Games.Mappers;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;

namespace FCG.Application.Games.UseCases;

public class UpdateGameUseCase(IGameRepository repository) : IUpdateGameUseCase
{
    public async Task<GameResponse> ExecuteAsync(Guid id, UpdateGameRequest request, CancellationToken ct = default)
    {
        var game = await repository.GetByIdAsync(id, ct)
                   ?? throw new GameNotFoundException(id);

        game.Update(
            title: request.Title,
            description: request.Description,
            price: request.Price,
            genre: request.Genre,
            releaseDate: request.ReleaseDate,
            status: request.Status);

        await repository.UpdateAsync(game, ct);

        return GameMapper.ToResponse(game);
    }
}
