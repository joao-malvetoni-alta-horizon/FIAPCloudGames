using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Exceptions;
using FluentAssertions;

namespace FCG.Tests.Domain.Entities;

public class UserOwnedGameTests
{
    private static readonly Guid ValidUserId = Guid.NewGuid();
    private static readonly Guid ValidGameId = Guid.NewGuid();

    [Fact]
    public void Create_ValidParams_ShouldReturnEntry()
    {
        var entry = UserOwnedGame.Create(ValidUserId, ValidGameId, 59.99m);

        entry.Should().NotBeNull();
        entry.UserId.Should().Be(ValidUserId);
        entry.GameId.Should().Be(ValidGameId);
        entry.PricePaid.Should().Be(59.99m);
        entry.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_EmptyUserId_ShouldThrowUserDomainException()
    {
        var act = () => UserOwnedGame.Create(Guid.Empty, ValidGameId, 0m);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*UserId cannot be an empty Guid*");
    }

    [Fact]
    public void Create_EmptyGameId_ShouldThrowUserDomainException()
    {
        var act = () => UserOwnedGame.Create(ValidUserId, Guid.Empty, 0m);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*GameId cannot be an empty Guid*");
    }

    [Fact]
    public void Create_NegativePricePaid_ShouldThrowUserDomainException()
    {
        var act = () => UserOwnedGame.Create(ValidUserId, ValidGameId, -1m);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*PricePaid cannot be negative*");
    }

    [Fact]
    public void Create_ValidParams_AcquiredAtShouldBeUtc()
    {
        var entry = UserOwnedGame.Create(ValidUserId, ValidGameId, 0m);
        entry.AcquiredAt.Kind.Should().Be(DateTimeKind.Utc);
    }
}
