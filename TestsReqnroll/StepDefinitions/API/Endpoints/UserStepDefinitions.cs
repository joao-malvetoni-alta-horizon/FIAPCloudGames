using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using FCG.API.Endpoints;
using FCG.API.Middlewares;
using FCG.Application.Users.DTOs;
using FCG.Application.Users.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.TestsReqnroll.StepDefinitions.API.Endpoints;

[Binding]
[Scope(Feature = "User Endpoints")] 
public class UserStepDefinitions
{
    private Guid _userId;
    private Guid _gameId;
    private HttpResponseMessage _response = null!;
    private readonly Mock<IPurchaseOwnedGameUseCase> _useCaseMock = new();
    private Guid? _authenticatedUserId;

    [Given(@"que o usuário está autenticado")]
    public void GivenQueOUsuarioEstaAutenticado()
    {
        _userId = Guid.NewGuid();
        _gameId = Guid.NewGuid();
        _authenticatedUserId = _userId;

        var expectedResponse = new PurchaseOwnedGameResponse(Guid.NewGuid(), _userId, _gameId, 100m, DateTime.UtcNow);

        _useCaseMock
            .Setup(u => u.ExecuteAsync(_userId, It.IsAny<PurchaseOwnedGameRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
    }

    [Given(@"que o usuário não está autenticado")]
    public void GivenQueOUsuarioNaoEstaAutenticado()
    {
        _authenticatedUserId = null;
    }

    [When(@"eu realizar uma requisição POST para ""(.*)""")]
    public async Task WhenEuRealizarUmaRequisicaoPOSTPara(string endpoint)
    {
        var app = BuildTestApp(_useCaseMock.Object, _authenticatedUserId);
        await using (app)
        {
            await app.StartAsync();
            var client = app.GetTestClient();

            var requestGameId = _authenticatedUserId.HasValue ? _gameId : Guid.NewGuid();

            _response = await client.PostAsJsonAsync(endpoint, new PurchaseOwnedGameRequest(requestGameId));
        }
    }

    [Then(@"o status code da resposta deve ser (.*)")]
    public void ThenOStatusCodeDaRespostaDeveSer(int statusCode)
    {
        _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

    [Then(@"o UseCase deve ser chamado uma única vez com o UserId correto")]
    public void ThenOUseCaseDeveSerChamadoUmaUnicaVezComOUserIdCorreto()
    {
        _useCaseMock.Verify(
            u => u.ExecuteAsync(_userId, It.IsAny<PurchaseOwnedGameRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static WebApplication BuildTestApp(IPurchaseOwnedGameUseCase useCase, Guid? authenticatedUserId)
    {
        var getOwnedGamesMock = new Mock<IGetUserOwnedGamesUseCase>();

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton(useCase);
        builder.Services.AddSingleton(getOwnedGamesMock.Object);
        builder.Services.AddSingleton(new TestAuthContext(authenticatedUserId));
        builder.Services
            .AddAuthentication("Test")
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
        builder.Services.AddAuthorization();

        var app = builder.Build();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapUserEndpoints();
        return app;
    }
}

internal sealed record TestAuthContext(Guid? UserId);

internal sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    TestAuthContext context) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (context.UserId is null)
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, context.UserId.Value.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}