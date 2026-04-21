using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Games.Policies;
using FCG.Domain.Shared;

namespace FCG.Application.Games.UseCases;

public class UpdatePromotionUseCase(
    IGamePromotionRepository promotionRepository,
    IUnitOfWork unitOfWork) : IUpdatePromotionUseCase
{
    public async Task<PromotionResponse> ExecuteAsync(Guid promotionId, UpdatePromotionRequest request, Guid roleId, CancellationToken ct = default)
    {
        GameManagementPolicy.EnsureCanManage(roleId);

        var promotion = await promotionRepository.GetByIdAsync(promotionId, ct)
            ?? throw new PromotionNotFoundException(promotionId);

        var newStart = request.StartDate ?? promotion.StartDate;
        var newEnd = request.EndDate ?? promotion.EndDate;
        var newIsActive = request.IsActive ?? promotion.IsActive;

        if (newIsActive && (request.StartDate.HasValue || request.EndDate.HasValue))
        {
            var hasOverlap = await promotionRepository.HasOverlappingActivePromotionAsync(
                promotion.GameId, newStart, newEnd, excludeId: promotion.Id, ct: ct);

            if (hasOverlap)
                throw new OverlappingPromotionException();
        }

        promotion.Update(request.DiscountType, request.DiscountValue, request.StartDate, request.EndDate, request.IsActive);

        promotionRepository.Update(promotion);
        await unitOfWork.CommitAsync(ct);

        return CreatePromotionUseCase.ToResponse(promotion);
    }
}