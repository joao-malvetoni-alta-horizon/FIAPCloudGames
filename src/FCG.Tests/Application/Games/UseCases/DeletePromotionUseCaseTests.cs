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

public class DeletePromotionUseCaseTests
{
    private static readonly Guid AdminRoleId = RoleType.Administrator.ToRoleId();
    private static readonly Guid UserRoleId = RoleType.User.ToRoleId();
    private static readonly DateTime Start = DateTime.UtcNow.AddDays(1);
    private static readonly DateTime End = DateTime.UtcNow.AddDays(10);

    private readonly Mock<IGamePromotionRepository> _promoRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private DeletePromotionUseCase CreateUseCase() =>
        new(_promoRepoMock.Object, _uowMock.Object);

    [Fact]
    public async Task ExecuteAsync_WhenNotAdmin_ShouldThrowInsufficientPermission()
    {
        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(Guid.NewGuid(), UserRoleId);

        await act.Should().ThrowAsync<InsufficientGameManagementPermissionException>();
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPromotionNotFound_ShouldThrowPromotionNotFoundException()
    {
        var missingId = Guid.NewGuid();
        _promoRepoMock.Setup(r => r.GetByIdAsync(missingId, It.IsAny<CancellationToken>())).ReturnsAsync((GamePromotion?)null);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(missingId, AdminRoleId);

        await act.Should().ThrowAsync<PromotionNotFoundException>();
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValid_ShouldDeleteAndCommit()
    {
        var promo = GamePromotion.Create(Guid.NewGuid(), DiscountType.Percentage, 15, Start, End);
        _promoRepoMock.Setup(r => r.GetByIdAsync(promo.Id, It.IsAny<CancellationToken>())).ReturnsAsync(promo);

        var useCase = CreateUseCase();
        await useCase.ExecuteAsync(promo.Id, AdminRoleId);

        _promoRepoMock.Verify(r => r.Delete(promo), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
