using FCG.Application.Games.DTOs;

namespace FCG.Application.Games.Interfaces;

public interface ICreatePromotionUseCase
{
    Task<PromotionResponse> ExecuteAsync(Guid gameId, CreatePromotionRequest request, Guid roleId, CancellationToken ct = default);
}