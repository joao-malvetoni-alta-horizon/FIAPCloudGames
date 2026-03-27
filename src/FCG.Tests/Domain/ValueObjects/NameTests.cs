using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.ValueObjects;
using FluentAssertions;

namespace FCG.Tests.Domain.ValueObjects;

public class NameTests
{
    [Fact]
    public void Create_ValidName_ShouldReturnName()
    {
        var name = Name.Create("John Doe");
        name.Value.Should().Be("John Doe");
    }

    [Fact]
    public void Create_EmptyName_ShouldThrowUserDomainException()
    {
        var act = () => Name.Create(string.Empty);
        act.Should().Throw<UserDomainException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void Create_WhitespaceName_ShouldThrowUserDomainException()
    {
        var act = () => Name.Create("   ");
        act.Should().Throw<UserDomainException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void Create_NameWithLeadingAndTrailingSpaces_ShouldTrim()
    {
        var name = Name.Create("  John Doe  ");

        name.Value.Should().Be("John Doe");
    }

    [Fact]
    public void Create_NameShorterThanMinLengthAfterTrim_ShouldThrowUserDomainException()
    {
        var act = () => Name.Create(" a ");

        act.Should().Throw<UserDomainException>()
            .WithMessage($"*between {Name.MinLength} and {Name.MaxLength}*");
    }

    [Fact]
    public void Create_NullName_ShouldThrowUserDomainException()
    {
        var act = () => Name.Create(null);

        act.Should().Throw<UserDomainException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void Names_WithSameNormalizedValue_ShouldBeEqual()
    {
        var first = Name.Create("John Doe");
        var second = Name.Create("  John Doe  ");

        first.Should().Be(second);
    }
}