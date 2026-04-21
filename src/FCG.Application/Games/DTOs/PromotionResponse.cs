using FCG.Domain.Games.Enums;

namespace FCG.Application.Games.DTOs;

public record PromotionResponse(
    Guid Id,
    Guid GameId,
    DiscountType DiscountType,
    decimal DiscountValue,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    bool IsCurrentlyValid,
    DateTime CreatedAt,
    DateTime? UpdatedAt);