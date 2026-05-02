using System.Net;
using System.Net.Http.Json;
using FCG.API.Endpoints;
using FCG.API.Middlewares;
using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FCG.TestsReqnroll.StepDefinitions.API.Endpoints;

[Binding]
[Scope(Feature = "Registro de Usuários")]
public class UsersStepDefinition
{
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private HttpResponseMessage _response = null!;
    private RegisterUserRequest _request = null!;

    [Given(@"que o serviço de usuário valida que o e-mail ""(.*)"" é único")]
    public void GivenQueOServicoDeUsuarioValidaQueOEmailEUnico(string email)
    {
        _userServiceMock
            .Setup(service => service.CheckEmailUniquenessAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Given(@"que o serviço de usuário informa que o e-mail ""(.*)"" já existe")]
    public void GivenQueOServicoDeUsuarioInformaQueOEmailJaExiste(string email)
    {
        _userServiceMock
            .Setup(service => service.CheckEmailUniquenessAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserAlreadyExistsException(email));
    }

    [Given(@"o hasher de senha está configurado")]
    public void GivenOHasherDeSenhaEstaConfigurado()
    {
        _passwordHasherMock
            .Setup(hasher => hasher.Hash(It.IsAny<string>()))
            .Returns("hashed-password");
    }

    [When(@"eu enviar uma requisição POST para ""(.*)"" com nome ""(.*)"", e-mail ""(.*)"" e senha ""(.*)""")]
    public async Task WhenEuEnviarUmaRequisicaoPOSTParaComDados(string endpoint, string name, string email, string password)
    {
        _request = new RegisterUserRequest(name, email, password);

        var app = BuildTestApp();
        await using (app)
        {
            await app.StartAsync();
            var client = app.GetTestClient();
            _response = await client.PostAsJsonAsync(endpoint, _request);
        }
    }

    [Then(@"o status code da resposta de registro deve ser (.*)")]
    public void ThenOStatusCodeDaRespostaDeRegistroDeveSer(int statusCode)
    {
        _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

    [Then(@"o corpo da resposta deve conter o e-mail ""(.*)""")]
    public async Task ThenOCorpoDaRespostaDeveConterOEmail(string expectedEmail)
    {
        var payload = await _response.Content.ReadFromJsonAsync<RegisterUserResponse>();
        payload.Should().NotBeNull();
        payload!.Email.Should().Be(expectedEmail);
    }

    private WebApplication BuildTestApp()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var unitOfWorkMock = new Mock<IUserUnitOfWork>();
        unitOfWorkMock.SetupGet(uow => uow.Users).Returns(userRepositoryMock.Object);
        unitOfWorkMock.Setup(uow => uow.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        builder.Services.AddScoped<RegisterUserUseCase>();
        builder.Services.AddSingleton(unitOfWorkMock.Object);
        builder.Services.AddSingleton(_passwordHasherMock.Object);
        builder.Services.AddSingleton(_userServiceMock.Object);

        var app = builder.Build();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.MapUsersEndpoints();
        return app;
    }
}