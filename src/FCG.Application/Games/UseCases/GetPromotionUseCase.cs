using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;

namespace FCG.Application.Games.UseCases;

public class GetPromotionUseCase(IGamePromotionRepository promotionRepository) : IGetPromotionUseCase
{
    public async Task<PromotionResponse> ExecuteAsync(Guid promotionId, CancellationToken ct = default)
    {
        var promotion = await promotionRepository.GetByIdAsync(promotionId, ct)
            ?? throw new PromotionNotFoundException(promotionId);

        return CreatePromotionUseCase.ToResponse(promotion);
    }
}