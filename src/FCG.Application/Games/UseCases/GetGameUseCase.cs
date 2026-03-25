using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Application.Games.Mappers;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;

namespace FCG.Application.Games.UseCases;

public class GetGameUseCase(IGameRepository repository) : IGetGameUseCase
{
    public async Task<GameResponse> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var game = await repository.GetByIdAsync(id, ct)
                   ?? throw new GameNotFoundException(id);

        return GameMapper.ToResponse(game);
    }
}