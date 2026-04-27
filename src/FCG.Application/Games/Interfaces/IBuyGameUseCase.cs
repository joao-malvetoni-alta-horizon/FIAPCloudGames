using FCG.Application.Games.DTOs;

namespace FCG.Application.Games.Interfaces;

/// <summary>
/// Interface para use case de compra de jogos
/// </summary>
public interface IBuyGameUseCase
{
    Task<BuyGameResponse> ExecuteAsync(BuyGameRequest request, Guid userId, CancellationToken ct = default);
}