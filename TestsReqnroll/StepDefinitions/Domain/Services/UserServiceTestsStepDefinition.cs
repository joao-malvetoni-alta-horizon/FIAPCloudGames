using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FCG.Domain.Users.Services;
using FluentAssertions;
using Moq;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
public class UserServiceTestsStepDefinition
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private UserService? _userService;
    private Func<Task>? _validationTask;

    [BeforeScenario]
    public void InitializeService()
    {
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Given(@"que o e-mail ""(.*)"" já está cadastrado no sistema")]
    public void GivenEmailJaCadastrado(string email)
    {
        _userRepositoryMock
            .Setup(repo => repo.ExistsByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    [When(@"eu validar a unicidade do e-mail ""(.*)""")]
    public void WhenValidarUnicidadeEmail(string email)
    {
        _validationTask = async () =>
            await _userService!.CheckEmailUniquenessAsync(email, CancellationToken.None);
    }

    [Then(@"deve ser lançada uma exceção informando que o usuário já existe com a mensagem contendo ""(.*)""")]
    public async Task ThenValidarExcecaoEmailDuplicado(string email)
    {
        await _validationTask.Should()
            .ThrowAsync<UserAlreadyExistsException>()
            .WithMessage($"*{email}*");
    }
}