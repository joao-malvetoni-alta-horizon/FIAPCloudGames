namespace FCG.Application.Games.Interfaces;

public interface IDeletePromotionUseCase
{
    Task ExecuteAsync(Guid promotionId, Guid roleId, CancellationToken ct = default);
}