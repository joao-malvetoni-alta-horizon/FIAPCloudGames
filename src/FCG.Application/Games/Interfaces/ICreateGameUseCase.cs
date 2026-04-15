using FCG.Application.Games.DTOs;

namespace FCG.Application.Games.Interfaces;

public interface ICreateGameUseCase
{
    Task<GameResponse> ExecuteAsync(CreateGameRequest request, Guid roleId, CancellationToken ct = default);
}
