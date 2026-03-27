using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.ValueObjects;
using FluentAssertions;

namespace FCG.Tests.Domain.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_ValidEmail_ShouldSucceed()
    {
        var email = Email.Create("user@example.com");
        email.Address.Should().Be("user@example.com");
    }

    [Fact]
    public void Create_ValidEmailWithSubdomain_ShouldSucceed()
    {
        var email = Email.Create("user@mail.example.com");
        email.Address.Should().Be("user@mail.example.com");
    }

    [Fact]
    public void Create_EmptyEmail_ShouldThrowUserDomainException()
    {
        var act = () => Email.Create(string.Empty);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*null or empty*");
    }

    [Fact]
    public void Create_NullEmail_ShouldThrowUserDomainException()
    {
        var act = () => Email.Create(null!);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*null or empty*");
    }

    [Fact]
    public void Create_EmailWithoutAt_ShouldThrowUserDomainException()
    {
        var act = () => Email.Create("userexample.com");
        act.Should().Throw<UserDomainException>()
           .WithMessage("*invalid format*");
    }

    [Fact]
    public void Create_EmailWithoutDomain_ShouldThrowUserDomainException()
    {
        var act = () => Email.Create("user@");
        act.Should().Throw<UserDomainException>()
           .WithMessage("*invalid format*");
    }

    [Fact]
    public void Create_EmailWithSpaces_ShouldThrowUserDomainException()
    {
        var act = () => Email.Create("user @example.com");
        act.Should().Throw<UserDomainException>()
           .WithMessage("*invalid format*");
    }

    [Fact]
    public void Equals_SameEmail_ShouldReturnTrue()
    {
        var e1 = Email.Create("user@example.com");
        var e2 = Email.Create("USER@EXAMPLE.COM");
        e1.Should().Be(e2);
    }

    [Fact]
    public void Equals_DifferentEmail_ShouldReturnFalse()
    {
        var e1 = Email.Create("a@example.com");
        var e2 = Email.Create("b@example.com");
        e1.Should().NotBe(e2);
    }

    [Fact]
    public void ImplicitConversion_StringToEmail_ShouldSucceed()
    {
        Email email = "user@example.com";
        string address = email;
        address.Should().Be("user@example.com");
    }
}
