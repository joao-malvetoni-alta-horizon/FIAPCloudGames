using FCG.Application.Games.UseCases;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Games.UseCases;

public class GetPromotionUseCaseTests
{
    private static readonly DateTime Start = DateTime.UtcNow.AddDays(1);
    private static readonly DateTime End = DateTime.UtcNow.AddDays(10);

    private readonly Mock<IGamePromotionRepository> _promoRepoMock = new();

    private GetPromotionUseCase CreateUseCase() => new(_promoRepoMock.Object);

    private static GamePromotion MakePromotion() =>
        GamePromotion.Create(Guid.NewGuid(), DiscountType.Percentage, 20, Start, End);

    [Fact]
    public async Task ExecuteAsync_WhenPromotionNotFound_ShouldThrowPromotionNotFoundException()
    {
        var missingId = Guid.NewGuid();
        _promoRepoMock.Setup(r => r.GetByIdAsync(missingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GamePromotion?)null);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(missingId);

        await act.Should().ThrowAsync<PromotionNotFoundException>()
            .WithMessage($"*{missingId}*");
    }

    [Fact]
    public async Task ExecuteAsync_WhenPromotionExists_ShouldReturnMappedResponse()
    {
        var promo = MakePromotion();
        _promoRepoMock.Setup(r => r.GetByIdAsync(promo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promo);

        var useCase = CreateUseCase();
        var response = await useCase.ExecuteAsync(promo.Id);

        response.Id.Should().Be(promo.Id);
        response.GameId.Should().Be(promo.GameId);
        response.DiscountType.Should().Be(DiscountType.Percentage);
        response.DiscountValue.Should().Be(20);
        response.IsActive.Should().BeTrue();
    }
}
