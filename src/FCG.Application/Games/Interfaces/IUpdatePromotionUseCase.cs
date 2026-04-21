using FCG.Application.Games.DTOs;

namespace FCG.Application.Games.Interfaces;

public interface IUpdatePromotionUseCase
{
    Task<PromotionResponse> ExecuteAsync(Guid promotionId, UpdatePromotionRequest request, Guid roleId, CancellationToken ct = default);
}