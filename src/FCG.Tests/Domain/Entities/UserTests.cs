using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FluentAssertions;

namespace FCG.Tests.Domain.Entities;

public class UserTests
{
    private static readonly Guid ValidRoleId = RoleType.User.ToRoleId();
    private const string ValidEmail = "user@example.com";
    private const string ValidHash = "$2a$12$somehashvalue";

    [Fact]
    public void Create_ValidParams_ShouldReturnUser()
    {
        var user = User.Create("John Doe", ValidEmail, ValidHash, ValidRoleId);

        user.Should().NotBeNull();
        user.Name.Should().Be("John Doe");
        user.Email.Address.Should().Be(ValidEmail);
        user.PasswordHash.Should().Be(ValidHash);
        user.RoleId.Should().Be(ValidRoleId);
        user.IsActive.Should().BeTrue();
        user.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_EmptyName_ShouldThrowUserDomainException()
    {
        var act = () => User.Create(string.Empty, ValidEmail, ValidHash, ValidRoleId);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*name cannot be null or empty*");
    }

    [Fact]
    public void Create_WhitespaceName_ShouldThrowUserDomainException()
    {
        var act = () => User.Create("   ", ValidEmail, ValidHash, ValidRoleId);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*name cannot be null or empty*");
    }

    [Fact]
    public void Create_InvalidEmail_ShouldThrowUserDomainException()
    {
        var act = () => User.Create("John", "notanemail", ValidHash, ValidRoleId);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*invalid format*");
    }

    [Fact]
    public void Create_EmptyRoleId_ShouldThrowUserDomainException()
    {
        var act = () => User.Create("John", ValidEmail, ValidHash, Guid.Empty);
        act.Should().Throw<UserDomainException>()
           .WithMessage("*RoleId cannot be an empty Guid*");
    }

    [Fact]
    public void UpdateName_ValidName_ShouldChangeName()
    {
        var user = User.Create("John Doe", ValidEmail, ValidHash, ValidRoleId);
        user.UpdateName("Jane Doe");

        user.Name.Should().Be("Jane Doe");
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var user = User.Create("John Doe", ValidEmail, ValidHash, ValidRoleId);
        user.Deactivate();

        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().NotBeNull();
    }
}
