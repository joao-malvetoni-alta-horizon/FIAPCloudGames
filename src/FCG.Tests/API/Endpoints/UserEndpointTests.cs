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

namespace FCG.Tests.API.Endpoints;

public class UserEndpointTests
{
    [Fact]
    public async Task PostPurchaseOwnedGame_WhenAuthenticated_ShouldPassUserIdFromJwtToUseCase()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var expectedResponse = new PurchaseOwnedGameResponse(Guid.NewGuid(), userId, gameId, 100m, DateTime.UtcNow);

        var useCaseMock = new Mock<IPurchaseOwnedGameUseCase>();
        useCaseMock
            .Setup(u => u.ExecuteAsync(userId, It.IsAny<PurchaseOwnedGameRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var app = BuildTestApp(useCaseMock.Object, authenticatedUserId: userId);
        await using (app)
        {
            await app.StartAsync();
            var client = app.GetTestClient();

            var response = await client.PostAsJsonAsync("/api/users/owned-games",
                new PurchaseOwnedGameRequest(gameId));

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            useCaseMock.Verify(
                u => u.ExecuteAsync(userId, It.IsAny<PurchaseOwnedGameRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    [Fact]
    public async Task PostPurchaseOwnedGame_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        var useCaseMock = new Mock<IPurchaseOwnedGameUseCase>();

        var app = BuildTestApp(useCaseMock.Object, authenticatedUserId: null);
        await using (app)
        {
            await app.StartAsync();
            var client = app.GetTestClient();

            var response = await client.PostAsJsonAsync("/api/users/owned-games",
                new PurchaseOwnedGameRequest(Guid.NewGuid()));

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
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
