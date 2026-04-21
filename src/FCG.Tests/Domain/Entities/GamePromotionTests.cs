using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FluentAssertions;

namespace FCG.Tests.Domain.Entities;

public class GamePromotionTests
{
    private static readonly Guid ValidGameId = Guid.NewGuid();
    private static readonly DateTime ValidStart = DateTime.UtcNow.AddDays(1);
    private static readonly DateTime ValidEnd = DateTime.UtcNow.AddDays(10);

    [Fact]
    public void Create_ValidParams_ShouldReturnActivePromotion()
    {
        var promo = GamePromotion.Create(ValidGameId, DiscountType.Percentage, 20, ValidStart, ValidEnd);

        promo.GameId.Should().Be(ValidGameId);
        promo.DiscountType.Should().Be(DiscountType.Percentage);
        promo.DiscountValue.Should().Be(20);
        promo.StartDate.Should().Be(ValidStart);
        promo.EndDate.Should().Be(ValidEnd);
        promo.IsActive.Should().BeTrue();
        promo.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_EmptyGameId_ShouldThrowDomainValidationException()
    {
        var act = () => GamePromotion.Create(Guid.Empty, DiscountType.Percentage, 10, ValidStart, ValidEnd);
        act.Should().Throw<DomainValidationException>().WithMessage("*GameId*");
    }

    [Fact]
    public void Create_ZeroDiscountValue_ShouldThrowDomainValidationException()
    {
        var act = () => GamePromotion.Create(ValidGameId, DiscountType.FixedValue, 0, ValidStart, ValidEnd);
        act.Should().Throw<DomainValidationException>().WithMessage("*greater than zero*");
    }

    [Fact]
    public void Create_NegativeDiscountValue_ShouldThrowDomainValidationException()
    {
        var act = () => GamePromotion.Create(ValidGameId, DiscountType.FixedValue, -5, ValidStart, ValidEnd);
        act.Should().Throw<DomainValidationException>().WithMessage("*greater than zero*");
    }

    [Fact]
    public void Create_PercentageOver100_ShouldThrowDomainValidationException()
    {
        var act = () => GamePromotion.Create(ValidGameId, DiscountType.Percentage, 101, ValidStart, ValidEnd);
        act.Should().Throw<DomainValidationException>().WithMessage("*100%*");
    }

    [Fact]
    public void Create_Percentage100_ShouldSucceed()
    {
        var promo = GamePromotion.Create(ValidGameId, DiscountType.Percentage, 100, ValidStart, ValidEnd);
        promo.DiscountValue.Should().Be(100);
    }

    [Fact]
    public void Create_StartDateAfterEndDate_ShouldThrowDomainValidationException()
    {
        var act = () => GamePromotion.Create(ValidGameId, DiscountType.Percentage, 10, ValidEnd, ValidStart);
        act.Should().Throw<DomainValidationException>().WithMessage("*before end date*");
    }

    [Fact]
    public void Create_StartDateEqualToEndDate_ShouldThrowDomainValidationException()
    {
        var sameDate = DateTime.UtcNow.AddDays(5);
        var act = () => GamePromotion.Create(ValidGameId, DiscountType.Percentage, 10, sameDate, sameDate);
        act.Should().Throw<DomainValidationException>().WithMessage("*before end date*");
    }

    [Fact]
    public void IsCurrentlyValid_ActiveAndWithinPeriod_ShouldReturnTrue()
    {
        var promo = GamePromotion.Create(
            ValidGameId, DiscountType.Percentage, 15,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1));

        promo.IsCurrentlyValid().Should().BeTrue();
    }

    [Fact]
    public void IsCurrentlyValid_ActiveButExpired_ShouldReturnFalse()
    {
        var promo = GamePromotion.Create(
            ValidGameId, DiscountType.Percentage, 15,
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(-1));

        promo.IsCurrentlyValid().Should().BeFalse();
    }

    [Fact]
    public void IsCurrentlyValid_ActiveButFuture_ShouldReturnFalse()
    {
        var promo = GamePromotion.Create(
            ValidGameId, DiscountType.Percentage, 15,
            DateTime.UtcNow.AddDays(5),
            DateTime.UtcNow.AddDays(10));

        promo.IsCurrentlyValid().Should().BeFalse();
    }

    [Fact]
    public void IsCurrentlyValid_InactiveWithinPeriod_ShouldReturnFalse()
    {
        var promo = GamePromotion.Create(
            ValidGameId, DiscountType.Percentage, 15,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1));

        promo.Deactivate();

        promo.IsCurrentlyValid().Should().BeFalse();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var promo = GamePromotion.Create(ValidGameId, DiscountType.Percentage, 10, ValidStart, ValidEnd);
        promo.Deactivate();

        promo.IsActive.Should().BeFalse();
        promo.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_ValidNewValues_ShouldApplyChanges()
    {
        var promo = GamePromotion.Create(ValidGameId, DiscountType.Percentage, 10, ValidStart, ValidEnd);
        var newEnd = ValidEnd.AddDays(5);

        promo.Update(DiscountType.FixedValue, 5.99m, null, newEnd, null);

        promo.DiscountType.Should().Be(DiscountType.FixedValue);
        promo.DiscountValue.Should().Be(5.99m);
        promo.EndDate.Should().Be(newEnd);
        promo.StartDate.Should().Be(ValidStart);
        promo.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_IsActiveFalse_ShouldDeactivate()
    {
        var promo = GamePromotion.Create(ValidGameId, DiscountType.Percentage, 10, ValidStart, ValidEnd);
        promo.Update(null, null, null, null, false);

        promo.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Update_InvalidNewValue_ShouldThrowDomainValidationException()
    {
        var promo = GamePromotion.Create(ValidGameId, DiscountType.Percentage, 10, ValidStart, ValidEnd);

        var act = () => promo.Update(DiscountType.Percentage, 150, null, null, null);

        act.Should().Throw<DomainValidationException>().WithMessage("*100%*");
    }
}