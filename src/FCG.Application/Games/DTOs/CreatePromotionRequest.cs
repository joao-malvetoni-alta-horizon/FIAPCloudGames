using FCG.Domain.Games.Enums;

namespace FCG.Application.Games.DTOs;

public record CreatePromotionRequest(
    DiscountType DiscountType,
    decimal DiscountValue,
    DateTime StartDate,
    DateTime EndDate);