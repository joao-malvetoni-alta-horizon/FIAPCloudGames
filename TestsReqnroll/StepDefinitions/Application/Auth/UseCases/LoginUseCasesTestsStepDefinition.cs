using FCG.Application.Auth.DTOs;
using FCG.Application.Auth.Interfaces;
using FCG.Application.Auth.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.TestsReqnroll.Application.Auth.UseCases;

[Binding]
[Scope(Feature = "Autenticação de Usuários")]
public class LoginUseCasesTestsStepDefinition
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly LoginUseCase _sut;

    private User? _currentUser;
    private LoginResponse? _response;
    private Func<Task>? _action;

    public LoginUseCasesTestsStepDefinition()
    {
        _sut = new LoginUseCase(_userRepositoryMock.Object, _passwordHasherMock.Object, _jwtTokenServiceMock.Object);
    }

    [Given(@"que não existe um usuário cadastrado com o e-mail ""(.*)""")]
    public void GivenQueNaoExisteUmUsuarioCadastrado(string email)
    {
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
    }

    [Given(@"que existe um usuário cadastrado com o e-mail ""(.*)""")]
    public void GivenQueExisteUmUsuarioCadastrado(string email)
    {
        _currentUser = User.Create("Test User", email, "$2a$12$somehashvalue", RoleType.User.ToRoleId());

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_currentUser);
    }

    [Given(@"esse usuário está inativo")]
    public void GivenEsseUsuarioEstaInativo()
    {
        _currentUser?.Deactivate();
    }

    [Given(@"que existe um usuário ativo cadastrado com e-mail ""(.*)"" e senha ""(.*)""")]
    public void GivenQueExisteUmUsuarioAtivo(string email, string password)
    {
        _currentUser = User.Create("Test User", email, "$2a$12$somehashvalue", RoleType.User.ToRoleId());

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_currentUser);

        _passwordHasherMock
            .Setup(h => h.Verify(password, _currentUser.PasswordHash))
            .Returns(true);
    }

    [When(@"eu tentar realizar o login com e-mail ""(.*)"" e senha ""(.*)""")]
    public async Task WhenEuTentarRealizarOLogin(string email, string password)
    {
        var request = new LoginRequest(email, password);

        if (_passwordHasherMock.Invocations.All(i => i.Method.Name != "Verify"))
        {
            _passwordHasherMock
               .Setup(h => h.Verify(It.Is<string>(s => s != password), It.IsAny<string>()))
               .Returns(false);
        }

        _jwtTokenServiceMock
            .Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("jwt-token");

        _action = async () => _response = await _sut.ExecuteAsync(request, CancellationToken.None);

        try
        {
            await _action();
        }
        catch (Exception)
        {
            // Exceção capturada para ser validada no Then
        }
    }

    [Then(@"deve ser lançada uma exceção de credenciais inválidas")]
    public async Task ThenDeveSerLancadaUmaExcecaoDeCredenciaisInvalidas()
    {
        await _action.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Then(@"o serviço de token JWT não deve ser chamado")]
    public void ThenOServicoDeTokenJWTNaoDeveSerChamado()
    {
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Then(@"o token de acesso retornado deve ser ""(.*)""")]
    public void ThenOTokenDeAcessoRetornadoDeveSer(string token)
    {
        _response!.AccessToken.Should().Be(token);
    }

    [Then(@"o tipo do token deve ser ""(.*)""")]
    public void ThenOTipoDoTokenDeveSer(string type)
    {
        _response!.TokenType.Should().Be(type);
    }

    [Then(@"o tempo de expiração deve ser de (.*) segundos")]
    public void ThenOTempoDeExpiracaoDeveSerDeSegundos(int seconds)
    {
        _response!.ExpiresIn.Should().Be(seconds);
    }

    [Then(@"o serviço de token JWT deve ser chamado uma única vez")]
    public void ThenOServicoDeTokenJWTDeveSerChamadoUmaUnicaVez()
    {
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(_currentUser!), Times.Once);
    }
}