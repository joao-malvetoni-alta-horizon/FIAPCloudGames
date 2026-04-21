using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Shared;

namespace FCG.Domain.Games.Entities;

public class GamePromotion : Entity
{
    public Guid GameId { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }

    public Game? Game { get; private set; }

    protected GamePromotion() { }

    private GamePromotion(Guid gameId, DiscountType discountType, decimal discountValue, DateTime startDate, DateTime endDate)
    {
        GameId = gameId;
        DiscountType = discountType;
        DiscountValue = discountValue;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
    }

    public static GamePromotion Create(Guid gameId, DiscountType discountType, decimal discountValue, DateTime startDate, DateTime endDate)
    {
        if (gameId == Guid.Empty)
            throw new DomainValidationException("GameId cannot be empty.");

        Validate(discountType, discountValue, startDate, endDate);

        return new GamePromotion(gameId, discountType, discountValue, startDate, endDate);
    }

    public void Update(DiscountType? discountType, decimal? discountValue, DateTime? startDate, DateTime? endDate, bool? isActive)
    {
        var newType = discountType ?? DiscountType;
        var newValue = discountValue ?? DiscountValue;
        var newStart = startDate ?? StartDate;
        var newEnd = endDate ?? EndDate;

        Validate(newType, newValue, newStart, newEnd);

        DiscountType = newType;
        DiscountValue = newValue;
        StartDate = newStart;
        EndDate = newEnd;

        if (isActive.HasValue)
            IsActive = isActive.Value;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsCurrentlyValid() =>
        IsActive && DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;

    private static void Validate(DiscountType discountType, decimal discountValue, DateTime startDate, DateTime endDate)
    {
        if (discountValue <= 0)
            throw new DomainValidationException("Discount value must be greater than zero.");

        if (discountType == DiscountType.Percentage && discountValue > 100)
            throw new DomainValidationException("Percentage discount cannot exceed 100%.");

        if (startDate >= endDate)
            throw new DomainValidationException("Start date must be before end date.");
    }
}