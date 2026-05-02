using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.ValueObjects;
using FluentAssertions;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
[Scope(Feature = "Validação de Complexidade de Senha")]
public class PasswordStepDefinitions
{
    private Action? _validationAction;

    [When(@"eu validar a senha ""(.*)""")]
    public void WhenValidarSenha(string password)
    {
        _validationAction = () => Password.Validate(password);
    }

    [Then(@"o sistema não deve lançar nenhuma exceção")]
    public void ThenNaoDeveLancarExcecao()
    {
        _validationAction.Should().NotThrow();
    }

    [When(@"eu tentar validar a senha ""(.*)""")]
    public void WhenTentarValidarInvalida(string password)
    {
        string input = (password == "null") ? null! : password;
        _validationAction = () => Password.Validate(input);
    }

    [Then(@"deve ser lançada uma exceção de domínio de usuário com a mensagem ""(.*)""")]
    public void ThenValidarMensagemExcecao(string mensagem)
    {
        _validationAction.Should().Throw<UserDomainException>()
            .WithMessage($"*{mensagem}*");
    }
}