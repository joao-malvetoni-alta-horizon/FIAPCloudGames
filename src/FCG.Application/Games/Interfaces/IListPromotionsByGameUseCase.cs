using FCG.Application.Games.DTOs;

namespace FCG.Application.Games.Interfaces;

public interface IListPromotionsByGameUseCase
{
    Task<IReadOnlyList<PromotionResponse>> ExecuteAsync(Guid gameId, CancellationToken ct = default);
}