using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.ValueObjects;
using FluentAssertions;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
public class EmailTestsStepDefinition
{
    private Email? _email;
    private Email? _otherEmail;
    private Action? _createAction;

    [When(@"eu criar um e-mail com o endereço ""(.*)""")]
    public void WhenCriarEmail(string address)
    {
        _email = Email.Create(address);
    }

    [Then(@"o endereço deve ser armazenado como ""(.*)""")]
    public void ThenValidarEndereco(string expected)
    {
        _email!.Address.Should().Be(expected);
    }

    [When(@"eu tentar criar um e-mail com o valor ""(.*)""")]
    public void WhenTentarCriarInvalido(string valor)
    {
        string input = valor == "null" ? null! : valor;
        _createAction = () => Email.Create(input);
    }

    [Then(@"deve ser lançada uma exceção de domínio de usuário com a mensagem ""(.*)""")]
    public void ThenValidarMensagemExcecao(string mensagem)
    {
        _createAction.Should().Throw<UserDomainException>()
            .WithMessage($"*{mensagem}*");
    }

    [Given(@"que eu tenho o e-mail ""(.*)""")]
    public void GivenTenhoEmail(string address)
    {
        _email = Email.Create(address);
    }

    [Given(@"eu tenho outro e-mail ""(.*)""")]
    public void GivenTenhoOutroEmail(string address)
    {
        _otherEmail = Email.Create(address);
    }

    [Then(@"os dois e-mails devem ser considerados iguais")]
    public void ThenValidarIgualdade()
    {
        _email.Should().Be(_otherEmail);
    }

    [When(@"eu converter a string ""(.*)"" implicitamente para E-mail")]
    public void WhenConversaoImplicita(string address)
    {
        Email converted = address;
        _email = converted;
    }

    [Then(@"o resultado deve ser um objeto Email com o endereço correto")]
    public void ThenValidarConversao()
    {
        string addressString = _email!;
        addressString.Should().NotBeNull();
    }
}