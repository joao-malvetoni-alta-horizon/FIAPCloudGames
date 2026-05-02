using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FluentAssertions;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
public class GamePromotionStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private GamePromotion? _promotion;
    private Action? _invalidAction;
    private bool _isValidResult;

    public GamePromotionStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [When(@"eu criar uma promoção para um jogo com desconto de ""(.*)"" de (.*)")]
    public void WhenCriarPromocaoValida(DiscountType type, decimal value)
    {
        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(10);
        _promotion = GamePromotion.Create(Guid.NewGuid(), type, value, start, end);
    }

    [When(@"a data de início for amanhã e o término em (.*) dias")]
    public void WhenDefinirDatas(int days)
    {
    }

    [Then(@"a promoção deve ser criada com sucesso e estar ativa")]
    public void ThenPromocaoAtiva()
    {
        _promotion.Should().NotBeNull();
        _promotion!.IsActive.Should().BeTrue();
        _promotion.Id.Should().NotBeEmpty();
    }

    [When(@"eu tentar criar uma promoção com desconto de ""(.*)"" de (.*)")]
    public void WhenTentarCriarDescontoInvalido(DiscountType type, decimal value)
    {
        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(10);

        _invalidAction = () => GamePromotion.Create(Guid.NewGuid(), type, value, start, end);
    }

    [When(@"eu tentar criar uma promoção onde a data de início é após o término")]
    public void WhenTentarCriarDataInvalida()
    {
        var start = DateTime.UtcNow.AddDays(10);
        var end = DateTime.UtcNow.AddDays(1);

        _invalidAction = () => GamePromotion.Create(Guid.NewGuid(), DiscountType.Percentage, 10, start, end);
    }

    [Then(@"deve ser lançada uma exceção de validação(.*) com a mensagem ""(.*)""")]
    public void ThenValidarMensagemExcecao(string ignore, string message)
    {
        _invalidAction.Should().Throw<DomainValidationException>()
            .WithMessage($"*{message}*");
    }

    [Given(@"que existe uma promoção com início em ""(.*)"" dias e término em ""(.*)"" dias")]
    public void GivenPromocaoComDatas(int startDays, int endDays)
    {
        _promotion = GamePromotion.Create(
            Guid.NewGuid(),
            DiscountType.Percentage,
            15,
            DateTime.UtcNow.AddDays(startDays),
            DateTime.UtcNow.AddDays(endDays));
    }

    [Given(@"que existe uma promoção ativa")]
    public void GivenPromocaoAtiva()
    {
        _promotion = GamePromotion.Create(
            Guid.NewGuid(),
            DiscountType.Percentage,
            10,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1));
    }

    [When(@"eu verificar se a promoção é válida atualmente")]
    public void WhenVerificarValidade()
    {
        _isValidResult = _promotion!.IsCurrentlyValid();
    }

    [When(@"eu desativar a promoção")]
    public void WhenDesativar()
    {
        _promotion!.Deactivate();
    }

    [Then(@"o resultado deve ser ""(.*)""")]
    public void ThenResultadoValidade(bool expected)
    {
        _isValidResult.Should().Be(expected);
    }

    [Then(@"a promoção não deve mais ser considerada válida atualmente")]
    public void ThenNaoValida()
    {
        _promotion!.IsCurrentlyValid().Should().BeFalse();
    }

    [Then(@"o campo IsActive deve ser falso")]
    public void ThenIsActiveFalse()
    {
        _promotion!.IsActive.Should().BeFalse();
    }
}