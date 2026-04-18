namespace FCG.Application.Users.DTOs;

public sealed record PurchaseOwnedGameResponse(
    Guid Id,
    Guid UserId,
    Guid GameId,
    decimal PricePaid,
    DateTime AcquiredAt);
