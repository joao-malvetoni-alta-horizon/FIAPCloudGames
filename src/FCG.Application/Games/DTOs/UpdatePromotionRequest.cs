using FCG.Domain.Games.Enums;

namespace FCG.Application.Games.DTOs;

public record UpdatePromotionRequest(
    DiscountType? DiscountType,
    decimal? DiscountValue,
    DateTime? StartDate,
    DateTime? EndDate,
    bool? IsActive);