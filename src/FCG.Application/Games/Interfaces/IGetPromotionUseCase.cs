using FCG.Application.Games.DTOs;

namespace FCG.Application.Games.Interfaces;

public interface IGetPromotionUseCase
{
    Task<PromotionResponse> ExecuteAsync(Guid promotionId, CancellationToken ct = default);
}