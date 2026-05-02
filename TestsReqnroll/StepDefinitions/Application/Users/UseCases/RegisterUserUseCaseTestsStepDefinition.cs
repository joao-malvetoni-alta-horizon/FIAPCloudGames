using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.TestsReqnroll.Application.Users.UseCases;

[Binding]
[Scope(Feature = "Registro de Novo Usuário")]
public class RegisterUserStepDefinitions
{
    private readonly Mock<IUserUnitOfWork> _uowMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();

    private RegisterUserRequest? _request;
    private RegisterUserResponse? _response;
    private Func<Task>? _action;

    [Given(@"que o sistema de hashing de senhas está ativo")]
    public void GivenHashingAtivo()
    {
        _uowMock.SetupGet(uow => uow.Users).Returns(_userRepoMock.Object);
        _passwordHasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-password");
    }

    [Given(@"o serviço de domínio de usuários está disponível")]
    public void GivenServicoDisponivel() { /* Mock inicializado no construtor */ }

    [Given(@"que o e-mail ""(.*)"" ainda não está cadastrado")]
    [Given(@"que o e-mail ""(.*)"" está disponível")]
    public void GivenEmailDisponivel(string email)
    {
        _userServiceMock
            .Setup(s => s.CheckEmailUniquenessAsync(email, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Given(@"que o e-mail ""(.*)"" já está cadastrado no sistema")]
    public void GivenEmailJaExiste(string email)
    {
        _userServiceMock
            .Setup(s => s.CheckEmailUniquenessAsync(email, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserAlreadyExistsException(email));
    }

    [When(@"eu solicitar o registro com o nome ""(.*)"", e-mail ""(.*)"" e senha ""(.*)""")]
    public void WhenSolicitarRegistro(string nome, string email, string senha)
    {
        _request = new RegisterUserRequest(nome, email, senha);
        var useCase = new RegisterUserUseCase(_uowMock.Object, _passwordHasherMock.Object, _userServiceMock.Object);
        _action = async () => _response = await useCase.ExecuteAsync(_request, CancellationToken.None);
    }

    [Then(@"o sistema deve realizar o hash da senha")]
    public async Task ThenRealizaHash()
    {
        await _action!.Invoke();
        _passwordHasherMock.Verify(h => h.Hash(_request!.Password), Times.Once);
    }

    [Then(@"os dados do novo usuário devem ser salvos e persistidos")]
    public void ThenSalvaEPersiste()
    {
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Then(@"a resposta deve conter o e-mail ""(.*)"" e o nome ""(.*)""")]
    public void ThenValidaResposta(string email, string nome)
    {
        _response!.Email.Should().Be(email);
        _response.Name.Should().Be(nome);
    }

    [Then(@"o cargo padrão deve ser do tipo ""User""")]
    public void ThenCargoPadrao()
    {
        _response!.RoleId.Should().Be(RoleType.User.ToRoleId());
    }

    [Then(@"deve ser lançada uma exceção informando que o usuário já existe")]
    public async Task ThenErroUsuarioJaExiste()
    {
        await _action!.Should().ThrowAsync<UserAlreadyExistsException>();
    }

    [Then(@"deve ser lançada uma exceção de domínio de usuário")]
    public async Task ThenErroDominio()
    {
        await _action!.Should().ThrowAsync<UserDomainException>();
    }

    [Then(@"o sistema não deve realizar o hash da senha nem persistir os dados")]
    public void ThenNaoPersisteDados()
    {
        _passwordHasherMock.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _uowMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}