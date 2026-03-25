using FCG.Application.Games.DTOs;

namespace FCG.Application.Games.Interfaces;

public interface IUpdateGameUseCase
{
    Task<GameResponse> ExecuteAsync(Guid id, UpdateGameRequest request, CancellationToken ct = default);
}
