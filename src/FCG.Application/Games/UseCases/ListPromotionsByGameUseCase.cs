using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Domain.Games.Interfaces;

namespace FCG.Application.Games.UseCases;

public class ListPromotionsByGameUseCase(IGamePromotionRepository promotionRepository) : IListPromotionsByGameUseCase
{
    public async Task<IReadOnlyList<PromotionResponse>> ExecuteAsync(Guid gameId, CancellationToken ct = default)
    {
        var promotions = await promotionRepository.GetByGameIdAsync(gameId, ct);

        return promotions
            .OrderByDescending(p => p.StartDate)
            .Select(CreatePromotionUseCase.ToResponse)
            .ToList();
    }
}