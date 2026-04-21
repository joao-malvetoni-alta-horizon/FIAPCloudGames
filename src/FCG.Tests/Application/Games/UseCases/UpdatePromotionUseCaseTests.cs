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

public class UpdatePromotionUseCaseTests
{
    private static readonly Guid AdminRoleId = RoleType.Administrator.ToRoleId();
    private static readonly Guid UserRoleId = RoleType.User.ToRoleId();
    private static readonly DateTime Start = DateTime.UtcNow.AddDays(1);
    private static readonly DateTime End = DateTime.UtcNow.AddDays(10);

    private readonly Mock<IGamePromotionRepository> _promoRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private UpdatePromotionUseCase CreateUseCase() =>
        new(_promoRepoMock.Object, _uowMock.Object);

    private static GamePromotion MakePromotion() =>
        GamePromotion.Create(Guid.NewGuid(), DiscountType.Percentage, 10, Start, End);

    [Fact]
    public async Task ExecuteAsync_WhenNotAdmin_ShouldThrowInsufficientPermission()
    {
        var promo = MakePromotion();
        _promoRepoMock.Setup(r => r.GetByIdAsync(promo.Id, It.IsAny<CancellationToken>())).ReturnsAsync(promo);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(promo.Id, new UpdatePromotionRequest(null, null, null, null, null), UserRoleId);

        await act.Should().ThrowAsync<InsufficientGameManagementPermissionException>();
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPromotionNotFound_ShouldThrowPromotionNotFoundException()
    {
        var missingId = Guid.NewGuid();
        _promoRepoMock.Setup(r => r.GetByIdAsync(missingId, It.IsAny<CancellationToken>())).ReturnsAsync((GamePromotion?)null);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(missingId, new UpdatePromotionRequest(null, null, null, null, null), AdminRoleId);

        await act.Should().ThrowAsync<PromotionNotFoundException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenNewDatesOverlapAnotherPromotion_ShouldThrowOverlappingPromotionException()
    {
        var promo = MakePromotion();
        var newEnd = End.AddDays(5);
        _promoRepoMock.Setup(r => r.GetByIdAsync(promo.Id, It.IsAny<CancellationToken>())).ReturnsAsync(promo);
        _promoRepoMock
            .Setup(r => r.HasOverlappingActivePromotionAsync(promo.GameId, Start, newEnd, promo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(promo.Id, new UpdatePromotionRequest(null, null, null, newEnd, null), AdminRoleId);

        await act.Should().ThrowAsync<OverlappingPromotionException>();
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValid_ShouldUpdateAndCommit()
    {
        var promo = MakePromotion();
        _promoRepoMock.Setup(r => r.GetByIdAsync(promo.Id, It.IsAny<CancellationToken>())).ReturnsAsync(promo);
        _promoRepoMock
            .Setup(r => r.HasOverlappingActivePromotionAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var useCase = CreateUseCase();
        var response = await useCase.ExecuteAsync(promo.Id, new UpdatePromotionRequest(DiscountType.FixedValue, 5.99m, null, null, null), AdminRoleId);

        response.DiscountType.Should().Be(DiscountType.FixedValue);
        response.DiscountValue.Should().Be(5.99m);
        _promoRepoMock.Verify(r => r.Update(promo), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOnlyDeactivating_ShouldNotCheckOverlap()
    {
        var promo = MakePromotion();
        _promoRepoMock.Setup(r => r.GetByIdAsync(promo.Id, It.IsAny<CancellationToken>())).ReturnsAsync(promo);

        var useCase = CreateUseCase();
        var response = await useCase.ExecuteAsync(promo.Id, new UpdatePromotionRequest(null, null, null, null, false), AdminRoleId);

        response.IsActive.Should().BeFalse();
        _promoRepoMock.Verify(r => r.HasOverlappingActivePromotionAsync(
            It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
