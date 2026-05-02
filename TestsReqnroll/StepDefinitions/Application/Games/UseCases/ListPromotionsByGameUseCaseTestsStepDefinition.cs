using FCG.Application.Games.DTOs;
using FCG.Application.Games.UseCases;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.TestsReqnroll.Application.Games.UseCases;

[Binding]
[Scope(Feature = "Listagem de Promoções por Jogo")]
public class ListPromotionsByGameUseCaseTestsStepDefinition
{
    private readonly Mock<IGamePromotionRepository> _promoRepoMock = new();
    private readonly Guid _gameId = Guid.NewGuid();
    private List<PromotionResponse>? _response;
    private List<GamePromotion> _promotions = new();

    [Given(@"que um jogo com ID específico não possui promoções cadastradas")]
    public void GivenJogoSemPromocoes()
    {
        _promoRepoMock
            .Setup(r => r.GetByGameIdAsync(_gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GamePromotion>());
    }

    [Given(@"que existem promoções cadastradas para um jogo específico")]
    public void GivenExistemPromocoesParaOJogo()
    {
        var now = DateTime.UtcNow;
        _promotions = new List<GamePromotion>
        {
            GamePromotion.Create(_gameId, DiscountType.Percentage, 10, now.AddDays(1), now.AddDays(5)),
            GamePromotion.Create(_gameId, DiscountType.FixedValue, 5.99m, now.AddDays(6), now.AddDays(10))
        };

        _promoRepoMock
            .Setup(r => r.GetByGameIdAsync(_gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_promotions);
    }

    [Given(@"que existem promoções com diferentes datas de início para um jogo")]
    public void GivenPromocoesComDiferentesDatas()
    {
        var now = DateTime.UtcNow;
        var older = GamePromotion.Create(_gameId, DiscountType.Percentage, 10, now.AddDays(1), now.AddDays(5));
        var newer = GamePromotion.Create(_gameId, DiscountType.FixedValue, 15, now.AddDays(6), now.AddDays(10));

        _promoRepoMock
            .Setup(r => r.GetByGameIdAsync(_gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GamePromotion> { older, newer });
    }

    [When(@"eu solicitar a lista de promoções desse jogo")]
    public async Task WhenSolicitarLista()
    {
        var useCase = new ListPromotionsByGameUseCase(_promoRepoMock.Object);
        var result = await useCase.ExecuteAsync(_gameId);
        _response = result.ToList();
    }

    [Then(@"a lista retornada deve estar vazia")]
    public void ThenListaVazia()
    {
        _response.Should().BeEmpty();
    }

    [Then(@"a lista deve conter exatamente ""(.*)"" promoções")]
    public void ThenListaContemQuantidade(int quantidade)
    {
        _response.Should().HaveCount(quantidade);
    }

    [Then(@"todas as promoções devem pertencer ao jogo solicitado")]
    public void ThenPertencemAoJogo()
    {
        _response.Should().AllSatisfy(r => r.GameId.Should().Be(_gameId));
    }

    [Then(@"a promoção mais recente deve aparecer primeiro na lista")]
    public void ThenMaisRecentePrimeiro()
    {
        _response.Should().BeInDescendingOrder(p => p.StartDate);
    }
}