using FCG.Application.Auth.DTOs;
using FCG.Application.Auth.Interfaces;
using FCG.Application.Auth.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Auth.UseCases;

public class LoginUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly LoginUseCase _sut;

    public LoginUseCaseTests()
    {
        _sut = new LoginUseCase(_userRepositoryMock.Object, _passwordHasherMock.Object, _jwtTokenServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ShouldThrowInvalidCredentialsException()
    {
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("unknown@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var act = async () => await _sut.ExecuteAsync(new LoginRequest("unknown@test.com", "anypass"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsInactive_ShouldThrowInvalidCredentialsException()
    {
        var user = CreateInactiveUser();
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(user.Email.Address, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var act = async () => await _sut.ExecuteAsync(new LoginRequest(user.Email.Address, "anypass"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordIsWrong_ShouldThrowInvalidCredentialsException()
    {
        var user = CreateActiveUser();
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(user.Email.Address, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(h => h.Verify("wrongpass", user.PasswordHash))
            .Returns(false);

        var act = async () => await _sut.ExecuteAsync(new LoginRequest(user.Email.Address, "wrongpass"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCredentialsAreValid_ShouldReturnLoginResponse()
    {
        var user = CreateActiveUser();
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(user.Email.Address, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(h => h.Verify("correct@123", user.PasswordHash))
            .Returns(true);
        _jwtTokenServiceMock
            .Setup(j => j.GenerateToken(user))
            .Returns("jwt-token");

        var response = await _sut.ExecuteAsync(new LoginRequest(user.Email.Address, "correct@123"), CancellationToken.None);

        response.AccessToken.Should().Be("jwt-token");
        response.TokenType.Should().Be("Bearer");
        response.ExpiresIn.Should().Be(4 * 3600);
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(user), Times.Once);
    }

    private static User CreateActiveUser()
        => User.Create("Test User", "test@fcg.com", "$2a$12$somehashvalue", RoleType.User.ToRoleId());

    private static User CreateInactiveUser()
    {
        var user = User.Create("Inactive User", "inactive@fcg.com", "$2a$12$somehashvalue", RoleType.User.ToRoleId());
        user.Deactivate();
        return user;
    }
}