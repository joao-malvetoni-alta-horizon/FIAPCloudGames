using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Exceptions;
using FluentAssertions;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
[Scope(Feature = "Registro de Jogos na Biblioteca do Usuário")]
public class UserOwnedGameTestsStepDefinition
{
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validGameId = Guid.NewGuid();
    private UserOwnedGame? _userOwnedGame;
    private Action? _createAction;

    [When(@"eu registrar a compra de um jogo para o usuário com preço (.*)")]
    public void WhenRegistrarCompra(decimal price)
    {
        _userOwnedGame = UserOwnedGame.Create(_validUserId, _validGameId, price);
    }

    [Then(@"o registro deve conter o ID do usuário e do jogo")]
    public void ThenValidarIds()
    {
        _userOwnedGame.Should().NotBeNull();
        _userOwnedGame!.UserId.Should().Be(_validUserId);
        _userOwnedGame.GameId.Should().Be(_validGameId);
    }

    [Then(@"o preço pago deve ser (.*)")]
    public void ThenValidarPreco(decimal price)
    {
        _userOwnedGame!.PricePaid.Should().Be(price);
    }

    [Then(@"a data de aquisição deve estar no formato UTC")]
    public void ThenValidarDataUtc()
    {
        _userOwnedGame!.AcquiredAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [When(@"eu tentar registrar uma aquisição com ""(.*)"" inválido")]
    public void WhenTentarRegistrarInvalido(string campo)
    {
        _createAction = campo switch
        {
            "UserId" => () => UserOwnedGame.Create(Guid.Empty, _validGameId, 0m),
            "GameId" => () => UserOwnedGame.Create(_validUserId, Guid.Empty, 0m),
            "Preco" => () => UserOwnedGame.Create(_validUserId, _validGameId, -1m),
            _ => throw new ArgumentException("Campo não mapeado")
        };
    }

    [Then(@"deve ser lançada uma exceção de domínio de usuário com a mensagem ""(.*)""")]
    public void ThenValidarMensagemExcecao(string mensagem)
    {
        _createAction.Should().Throw<UserDomainException>()
            .WithMessage($"*{mensagem}*");
    }
}