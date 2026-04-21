using FCG.Application.Games.Interfaces;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Games.Policies;
using FCG.Domain.Shared;

namespace FCG.Application.Games.UseCases;

public class DeletePromotionUseCase(
    IGamePromotionRepository promotionRepository,
    IUnitOfWork unitOfWork) : IDeletePromotionUseCase
{
    public async Task ExecuteAsync(Guid promotionId, Guid roleId, CancellationToken ct = default)
    {
        GameManagementPolicy.EnsureCanManage(roleId);

        var promotion = await promotionRepository.GetByIdAsync(promotionId, ct)
            ?? throw new PromotionNotFoundException(promotionId);

        promotionRepository.Delete(promotion);
        await unitOfWork.CommitAsync(ct);
    }
}