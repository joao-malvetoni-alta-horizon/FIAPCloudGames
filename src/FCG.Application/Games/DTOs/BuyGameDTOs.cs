namespace FCG.Application.Games.DTOs;

/// <summary>
/// Request para compra de jogo
/// </summary>
public record BuyGameRequest(
    Guid GameId);

/// <summary>
/// Response para compra de jogo
/// </summary>
public record BuyGameResponse(
    Guid UserGameLibraryId,
    Guid UserId,
    Guid GameId,
    string GameTitle,
    decimal PricePaid,
    DateTime PurchasedAt);