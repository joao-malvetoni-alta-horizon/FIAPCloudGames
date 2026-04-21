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

namespace FCG.Tests.API.Endpoints;

public class AuthEndpointsTests
{
    [Fact]
    public async Task PostLogin_WhenCredentialsAreValid_ShouldReturnOkWithToken()
    {
        var loginResponse = new LoginResponse("jwt-token", "Bearer", 4 * 3600);
        var loginUseCaseMock = new Mock<ILoginUseCase>();
        loginUseCaseMock
            .Setup(u => u.ExecuteAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(loginResponse);

        var app = BuildTestApp(loginUseCaseMock.Object);
        await using (app)
        {
            await app.StartAsync();
            var client = app.GetTestClient();

            var response = await client.PostAsJsonAsync("/api/auth/login",
                new LoginRequest("user@fcg.com", "Valid@123"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
            payload.Should().NotBeNull();
            payload!.AccessToken.Should().Be("jwt-token");
            payload.TokenType.Should().Be("Bearer");
            payload.ExpiresIn.Should().Be(4 * 3600);
        }
    }

    [Fact]
    public async Task PostLogin_WhenCredentialsAreInvalid_ShouldReturnUnauthorized()
    {
        var loginUseCaseMock = new Mock<ILoginUseCase>();
        loginUseCaseMock
            .Setup(u => u.ExecuteAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidCredentialsException());

        var app = BuildTestApp(loginUseCaseMock.Object);
        await using (app)
        {
            await app.StartAsync();
            var client = app.GetTestClient();

            var response = await client.PostAsJsonAsync("/api/auth/login",
                new LoginRequest("user@fcg.com", "wrong"));

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    private static WebApplication BuildTestApp(ILoginUseCase loginUseCase)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton(loginUseCase);

        var app = builder.Build();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.MapAuthEndpoints();
        return app;
    }
}