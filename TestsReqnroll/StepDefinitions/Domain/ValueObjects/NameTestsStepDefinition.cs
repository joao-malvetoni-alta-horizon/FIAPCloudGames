using FCG.Domain.Users.ValueObjects;
using FCG.Domain.Users.Exceptions;
using FluentAssertions;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
public class NameTestsStepDefinitions
{
    private Name? _name;
    private Exception? _lastException;

    [When(@"eu tentar criar um nome com o valor (.*)")]
    public void WhenTentarCriarNome(string? valor)
    {
        string? input = valor?.Replace("\"", "");

        if (input == "null") input = null;

        _lastException = Record.Exception(() => _name = Name.Create(input!));
    }

    [Then(@"deve ser lançada uma exceção de domínio de usuário com a mensagem contendo ""(.*)""")]
    public void ThenValidarMensagemExcecao(string mensagemEsperada)
    {
        _lastException.Should().NotBeNull();
        _lastException.Should().BeOfType<UserDomainException>();

        _lastException!.Message.Should().MatchEquivalentOf($"*{mensagemEsperada}*");
    }

    [When(@"eu criar um nome com o valor ""(.*)""")]
    public void WhenCriarNomeValido(string valor)
    {
        _name = Name.Create(valor);
    }

    [Then(@"o valor do nome deve ser ""(.*)""")]
    public void ThenValidarValor(string esperado)
    {
        _name!.Value.Should().Be(esperado);
    }
}