using FCG.Application.Games.DTOs;

namespace FCG.Application.Games.Interfaces;

public interface IGetGameUseCase
{
    Task<GameResponse> ExecuteAsync(Guid id, CancellationToken ct = default);
}
