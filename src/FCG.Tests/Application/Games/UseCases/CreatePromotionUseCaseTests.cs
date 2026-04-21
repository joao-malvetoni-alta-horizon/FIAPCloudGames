using FCG.Application.Games.DTOs;
using FCG.Application.Games.UseCases;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Shared;
using FCG.Domain.Users.Enums;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Games.UseCases;

public class CreatePromotionUseCaseTests
{
    private static readonly Guid AdminRoleId = RoleType.Administrator.ToRoleId();
    private static readonly Guid UserRoleId = RoleType.User.ToRoleId();
    private static readonly DateTime Start = DateTime.UtcNow.AddDays(1);
    private static readonly DateTime End = DateTime.UtcNow.AddDays(10);

    private readonly Mock<IGameRepository> _gameRepoMock = new();
    private readonly Mock<IGamePromotionRepository> _promoRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private CreatePromotionUseCase CreateUseCase() =>
        new(_gameRepoMock.Object, _promoRepoMock.Object, _uowMock.Object);

    private static Game MakeGame()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        return new Game("Test Game", "Description", 29.99m, GameGenre.Action, future);
    }

    private CreatePromotionRequest ValidRequest() =>
        new(DiscountType.Percentage, 20, Start, End);

    [Fact]
    public async Task ExecuteAsync_WhenNotAdmin_ShouldThrowInsufficientPermission()
    {
        var game = MakeGame();
        _gameRepoMock.Setup(r => r.GetByIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(game);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(game.Id, ValidRequest(), UserRoleId);

        await act.Should().ThrowAsync<InsufficientGameManagementPermissionException>();
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGameNotFound_ShouldThrowGameNotFoundException()
    {
        var gameId = Guid.NewGuid();
        _gameRepoMock.Setup(r => r.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync((Game?)null);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(gameId, ValidRequest(), AdminRoleId);

        await act.Should().ThrowAsync<GameNotFoundException>();
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOverlappingPromotion_ShouldThrowOverlappingPromotionException()
    {
        var game = MakeGame();
        _gameRepoMock.Setup(r => r.GetByIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(game);
        _promoRepoMock
            .Setup(r => r.HasOverlappingActivePromotionAsync(game.Id, Start, End, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(game.Id, ValidRequest(), AdminRoleId);

        await act.Should().ThrowAsync<OverlappingPromotionException>();
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValid_ShouldCreatePromotionAndCommit()
    {
        var game = MakeGame();
        _gameRepoMock.Setup(r => r.GetByIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(game);
        _promoRepoMock
            .Setup(r => r.HasOverlappingActivePromotionAsync(game.Id, Start, End, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var useCase = CreateUseCase();
        var response = await useCase.ExecuteAsync(game.Id, ValidRequest(), AdminRoleId);

        response.GameId.Should().Be(game.Id);
        response.DiscountType.Should().Be(DiscountType.Percentage);
        response.DiscountValue.Should().Be(20);
        response.IsActive.Should().BeTrue();
        response.Id.Should().NotBeEmpty();
        _promoRepoMock.Verify(r => r.AddAsync(It.IsAny<GamePromotion>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
