using System.Net;
using System.Net.Http.Json;
using FCG.Application.Auth.DTOs;
using FCG.Application.Auth.Interfaces;
using FCG.API.Endpoints;
using FCG.API.Middlewares;
using FCG.Domain.Users.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FCG.TestsReqnroll.API.Endpoints;

[Binding]
[Scope(Feature = "Endpoints de Autenticação")] 
public class AuthStepDefinitions
{
    private readonly Mock<ILoginUseCase> _loginUseCaseMock = new();
    private HttpResponseMessage _response = null!;
    private LoginResponse? _loginPayload;

    
    [Given(@"que o caso de uso de login está configurado para retornar um token válido")]
    public void GivenQueOCasoDeUsoDeLoginEstaConfiguradoParaRetornarUmTokenValido()
    {
        var loginResponse = new LoginResponse("jwt-token", "Bearer", 4 * 3600);

        _loginUseCaseMock
            .Setup(u => u.ExecuteAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(loginResponse);
    }

    [Given(@"que o caso de uso de login está configurado para lançar erro de credenciais inválidas")]
    public void GivenQueOCasoDeUsoDeLoginEstaConfiguradoParaLancarErroDeCredenciaisInvalidas()
    {
        _loginUseCaseMock
            .Setup(u => u.ExecuteAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidCredentialsException());
    }

    [When(@"eu realizar uma requisição POST para ""(.*)"" com e-mail ""(.*)"" e senha ""(.*)""")]
    public async Task WhenEuRealizarUmaRequisicaoPOSTParaComCredenciais(string endpoint, string email, string password)
    {
        var app = BuildTestApp();
        await using (app)
        {
            await app.StartAsync();
            var client = app.GetTestClient();

            var request = new LoginRequest(email, password);
            _response = await client.PostAsJsonAsync(endpoint, request);
        }
    }

    [Then(@"o status code da resposta de auth deve ser (.*)")]
    public void ThenOStatusCodeDaRespostaDeAuthDeveSer(int statusCode)
    {
        _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

    [Then(@"o corpo da resposta deve conter o AccessToken ""(.*)""")]
    public async Task ThenOCorpoDaRespostaDeveConterOAccessToken(string expectedToken)
    {
        _loginPayload ??= await _response.Content.ReadFromJsonAsync<LoginResponse>();

        _loginPayload.Should().NotBeNull();
        _loginPayload!.AccessToken.Should().Be(expectedToken);
    }

    [Then(@"o tipo do token deve ser ""(.*)""")]
    public void ThenOTipoDoTokenDeveSer(string expectedType)
    {
        _loginPayload.Should().NotBeNull();
        _loginPayload!.TokenType.Should().Be(expectedType);
    }

    [Then(@"o tempo de expiração deve ser (.*)")]
    public void ThenOTempoDeExpiracaoDeveSer(int expectedExpires)
    {
        _loginPayload.Should().NotBeNull();
        _loginPayload!.ExpiresIn.Should().Be(expectedExpires);
    }

    private WebApplication BuildTestApp()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        builder.Services.AddSingleton(_loginUseCaseMock.Object);

        var app = builder.Build();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.MapAuthEndpoints();
        return app;
    }
}