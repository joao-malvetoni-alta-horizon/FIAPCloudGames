using FCG.Application.Games.DTOs;
using FCG.Application.Games.UseCases;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.TestsReqnroll.Application.Games.UseCases;

[Binding]
[Scope(Feature = "Consulta de Promoção de Jogos")]
public class GetPromotionUseCaseTestsStepDefinition
{
    private readonly Mock<IGamePromotionRepository> _promoRepoMock = new();
    private GamePromotion? _promo;
    private PromotionResponse? _response;
    private Func<Task>? _action;
    private Guid _searchId;

    [Given(@"que uma promoção com ID específico não existe no sistema")]
    public void GivenQueUmaPromocaoInexistente()
    {
        _searchId = Guid.NewGuid();
        _promoRepoMock
            .Setup(r => r.GetByIdAsync(_searchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GamePromotion?)null);
    }

    [Given(@"que existe uma promoção cadastrada no sistema")]
    public void GivenQueExisteUmaPromocaoCadastrada()
    {
        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(10);
        _promo = GamePromotion.Create(Guid.NewGuid(), DiscountType.Percentage, 20, start, end);
        _searchId = _promo.Id;

        _promoRepoMock
            .Setup(r => r.GetByIdAsync(_promo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_promo);
    }

    [When(@"eu buscar pelos detalhes dessa promoção")]
    public void WhenEuBuscarPelosDetalhes()
    {
        var useCase = new GetPromotionUseCase(_promoRepoMock.Object);
        _action = async () => _response = await useCase.ExecuteAsync(_searchId);
    }

    [Then(@"deve ser lançada uma exceção de promoção não encontrada")]
    public async Task ThenErroNaoEncontrada()
    {
        await _action!.Should().ThrowAsync<PromotionNotFoundException>()
            .WithMessage($"*{_searchId}*");
    }

    [Then(@"os detalhes da promoção devem ser retornados corretamente")]
    public async Task ThenDetalhesRetornadosCorretamente()
    {
        await _action!.Invoke();
        _response.Should().NotBeNull();
        _response!.Id.Should().Be(_promo!.Id);
        _response.GameId.Should().Be(_promo.GameId);
        _response.DiscountValue.Should().Be(20);
        _response.DiscountType.Should().Be(DiscountType.Percentage);
    }

    [Then(@"o status da promoção deve ser ativo")]
    public void ThenStatusAtivo()
    {
        _response!.IsActive.Should().BeTrue();
    }
}