using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.ValueObjects;
using FluentAssertions;

namespace FCG.Tests.Domain.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void Validate_ValidPassword_ShouldNotThrow()
    {
        var act = () => Password.Validate("StrongPass1!");
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_TooShort_ShouldThrowUserDomainException()
    {
        var act = () => Password.Validate("Ab1!");
        act.Should().Throw<UserDomainException>()
           .WithMessage("*at least 8 characters*");
    }

    [Fact]
    public void Validate_NoUppercase_ShouldThrowUserDomainException()
    {
        var act = () => Password.Validate("lowercase1!");
        act.Should().Throw<UserDomainException>()
           .WithMessage("*uppercase*");
    }

    [Fact]
    public void Validate_NoLowercase_ShouldThrowUserDomainException()
    {
        var act = () => Password.Validate("UPPERCASE1!");
        act.Should().Throw<UserDomainException>()
           .WithMessage("*lowercase*");
    }

    [Fact]
    public void Validate_NoDigit_ShouldThrowUserDomainException()
    {
        var act = () => Password.Validate("NoDigitPass!");
        act.Should().Throw<UserDomainException>()
           .WithMessage("*digit*");
    }

    [Fact]
    public void Validate_NoSpecialChar_ShouldThrowUserDomainException()
    {
        var act = () => Password.Validate("NoSpecial1A");
        act.Should().Throw<UserDomainException>()
           .WithMessage("*special character*");
    }

    [Fact]
    public void Validate_EmptyPassword_ShouldThrowUserDomainException()
    {
        var act = () => Password.Validate(string.Empty);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*null or empty*");
    }

    [Fact]
    public void Validate_NullPassword_ShouldThrowUserDomainException()
    {
        var act = () => Password.Validate(null!);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*null or empty*");
    }
}
