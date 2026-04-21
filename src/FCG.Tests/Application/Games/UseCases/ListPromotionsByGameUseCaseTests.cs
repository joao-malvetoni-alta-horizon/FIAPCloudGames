using FCG.Application.Games.UseCases;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Games.UseCases;

public class ListPromotionsByGameUseCaseTests
{
    private readonly Mock<IGamePromotionRepository> _promoRepoMock = new();

    private ListPromotionsByGameUseCase CreateUseCase() => new(_promoRepoMock.Object);

    [Fact]
    public async Task ExecuteAsync_WhenNoPromotions_ShouldReturnEmptyList()
    {
        var gameId = Guid.NewGuid();
        _promoRepoMock.Setup(r => r.GetByGameIdAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GamePromotion>());

        var useCase = CreateUseCase();
        var result = await useCase.ExecuteAsync(gameId);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WhenPromotionsExist_ShouldReturnMappedResponses()
    {
        var gameId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var promos = new List<GamePromotion>
        {
            GamePromotion.Create(gameId, DiscountType.Percentage, 10, now.AddDays(1), now.AddDays(5)),
            GamePromotion.Create(gameId, DiscountType.FixedValue, 5.99m, now.AddDays(6), now.AddDays(10))
        };
        _promoRepoMock.Setup(r => r.GetByGameIdAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promos);

        var useCase = CreateUseCase();
        var result = await useCase.ExecuteAsync(gameId);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(r => r.GameId.Should().Be(gameId));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnPromotionsOrderedByStartDateDescending()
    {
        var gameId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var older = GamePromotion.Create(gameId, DiscountType.Percentage, 10, now.AddDays(1), now.AddDays(5));
        var newer = GamePromotion.Create(gameId, DiscountType.FixedValue, 15, now.AddDays(6), now.AddDays(10));

        _promoRepoMock.Setup(r => r.GetByGameIdAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GamePromotion> { older, newer });

        var useCase = CreateUseCase();
        var result = await useCase.ExecuteAsync(gameId);

        result[0].StartDate.Should().BeAfter(result[1].StartDate);
    }
}
