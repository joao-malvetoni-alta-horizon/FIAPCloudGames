namespace FCG.Application.Users.DTOs;

public sealed record UserOwnedGameResponse(
    Guid Id,
    Guid UserId,
    Guid GameId,
    decimal PricePaid,
    DateTime AcquiredAt);
