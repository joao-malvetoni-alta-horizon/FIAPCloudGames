using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Users.UseCases;

public class PurchaseOwnedGameUseCaseTests
{
    private readonly Mock<IUserUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _usersMock = new();
    private readonly Mock<IUserOwnedGameRepository> _librariesMock = new();
    private readonly Mock<IGameRepository> _gamesMock = new();
    private readonly PurchaseOwnedGameUseCase _sut;

    public PurchaseOwnedGameUseCaseTests()
    {
        _unitOfWorkMock.SetupGet(u => u.Users).Returns(_usersMock.Object);
        _unitOfWorkMock.SetupGet(u => u.UserOwnedGames).Returns(_librariesMock.Object);
        _sut = new PurchaseOwnedGameUseCase(_unitOfWorkMock.Object, _gamesMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidPurchase_ShouldCreateOwnedGameAndCommit()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var user = CreateActiveUser();
        var game = CreateGame(price: 120m, status: GameStatus.Active);

        _usersMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _gamesMock.Setup(r => r.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync(game);
        _librariesMock.Setup(r => r.ExistsAsync(userId, gameId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var response = await _sut.ExecuteAsync(userId, new PurchaseOwnedGameRequest(gameId));

        response.UserId.Should().Be(userId);
        response.GameId.Should().Be(gameId);
        response.PricePaid.Should().Be(120m);

        _librariesMock.Verify(
            r => r.AddAsync(
                It.Is<UserOwnedGame>(g => g.UserId == userId && g.GameId == gameId && g.PricePaid == 120m),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowUserNotFoundException()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        _usersMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var act = async () => await _sut.ExecuteAsync(userId, new PurchaseOwnedGameRequest(gameId));

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsInactive_ShouldThrowUserDomainException()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var user = CreateActiveUser();
        user.Deactivate();

        _usersMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var act = async () => await _sut.ExecuteAsync(userId, new PurchaseOwnedGameRequest(gameId));

        await act.Should().ThrowAsync<UserDomainException>()
            .WithMessage("Inactive users cannot acquire games.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenGameIsNotActive_ShouldThrowDomainValidationException()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var user = CreateActiveUser();
        var game = CreateGame(price: 100m, status: GameStatus.ComingSoon);

        _usersMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _gamesMock.Setup(r => r.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync(game);

        var act = async () => await _sut.ExecuteAsync(userId, new PurchaseOwnedGameRequest(gameId));

        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("Only active games can be acquired.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserAlreadyOwnsGame_ShouldThrowUserAlreadyOwnsGameException()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var user = CreateActiveUser();
        var game = CreateGame(price: 100m, status: GameStatus.Active);

        _usersMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _gamesMock.Setup(r => r.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync(game);
        _librariesMock.Setup(r => r.ExistsAsync(userId, gameId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var act = async () => await _sut.ExecuteAsync(userId, new PurchaseOwnedGameRequest(gameId));

        await act.Should().ThrowAsync<UserAlreadyOwnsGameException>();
    }

    private static User CreateActiveUser()
        => User.Create("John Doe", "john@example.com", "$2a$12$hash", RoleType.User.ToRoleId());

    private static Game CreateGame(decimal price, GameStatus status)
    {
        var game = new Game("Cyber Runner", "Great game", price, GameGenre.Action,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));

        if (status != GameStatus.Active)
            game.Update(status: status);

        return game;
    }
}
