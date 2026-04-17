using FCG.Application.Users.DTOs;

namespace FCG.Application.Users.Interfaces;

public interface IPurchaseOwnedGameUseCase
{
    Task<PurchaseOwnedGameResponse> ExecuteAsync(
        Guid userId,
        PurchaseOwnedGameRequest request,
        CancellationToken cancellationToken = default);
}
