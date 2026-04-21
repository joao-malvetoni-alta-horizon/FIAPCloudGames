using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Games.Policies;
using FCG.Domain.Shared;

namespace FCG.Application.Games.UseCases;

public class CreatePromotionUseCase(
    IGameRepository gameRepository,
    IGamePromotionRepository promotionRepository,
    IUnitOfWork unitOfWork) : ICreatePromotionUseCase
{
    public async Task<PromotionResponse> ExecuteAsync(Guid gameId, CreatePromotionRequest request, Guid roleId, CancellationToken ct = default)
    {
        GameManagementPolicy.EnsureCanManage(roleId);

        var game = await gameRepository.GetByIdAsync(gameId, ct)
            ?? throw new GameNotFoundException(gameId);

        var hasOverlap = await promotionRepository.HasOverlappingActivePromotionAsync(
            game.Id, request.StartDate, request.EndDate, ct: ct);

        if (hasOverlap)
            throw new OverlappingPromotionException();

        var promotion = GamePromotion.Create(game.Id, request.DiscountType, request.DiscountValue, request.StartDate, request.EndDate);

        await promotionRepository.AddAsync(promotion, ct);
        await unitOfWork.CommitAsync(ct);

        return ToResponse(promotion);
    }

    internal static PromotionResponse ToResponse(GamePromotion p) =>
        new(p.Id, p.GameId, p.DiscountType, p.DiscountValue, p.StartDate, p.EndDate, p.IsActive, p.IsCurrentlyValid(), p.CreatedAt, p.UpdatedAt);
}