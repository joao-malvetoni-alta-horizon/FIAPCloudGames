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

namespace FCG.Tests.API.Endpoints;

public class UsersEndpointTests
{
    [Fact]
    public async Task PostRegister_WhenRequestIsValid_ShouldReturnCreated()
    {
        // Arrange
        var app = BuildTestApp(
            configureUserService: mock =>
                mock.Setup(service => service.CheckEmailUniquenessAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask),
            configurePasswordHasher: mock =>
                mock.Setup(hasher => hasher.Hash(It.IsAny<string>())).Returns("hashed-password"));
        await using (app)
        {
            await app.StartAsync();
            var client = app.GetTestClient();

            // Act
            var response = await client.PostAsJsonAsync("/api/users/register",
                new RegisterUserRequest("User Name", "user@example.com", "Strong@123"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var payload = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
            payload.Should().NotBeNull();
            payload!.Email.Should().Be("user@example.com");
        }
    }

    [Fact]
    public async Task PostRegister_WhenEmailAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        var app = BuildTestApp(
            configureUserService: mock =>
                mock.Setup(service => service.CheckEmailUniquenessAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new UserAlreadyExistsException("user@example.com")),
            configurePasswordHasher: mock =>
                mock.Setup(hasher => hasher.Hash(It.IsAny<string>())).Returns("hashed-password"));
        await using (app)
        {
            await app.StartAsync();
            var client = app.GetTestClient();

            // Act
            var response = await client.PostAsJsonAsync("/api/users/register",
                new RegisterUserRequest("User Name", "user@example.com", "Strong@123"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }

    [Fact]
    public async Task PostRegister_WhenPasswordIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var app = BuildTestApp(
            configureUserService: mock =>
                mock.Setup(service => service.CheckEmailUniquenessAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask),
            configurePasswordHasher: mock =>
                mock.Setup(hasher => hasher.Hash(It.IsAny<string>())).Returns("hashed-password"));
        await using (app)
        {
            await app.StartAsync();
            var client = app.GetTestClient();

            // Act
            var response = await client.PostAsJsonAsync("/api/users/register",
                new RegisterUserRequest("User Name", "user@example.com", "abc"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    private static WebApplication BuildTestApp(
        Action<Mock<IUserService>> configureUserService,
        Action<Mock<IPasswordHasher>> configurePasswordHasher)
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var unitOfWorkMock = new Mock<IUserUnitOfWork>();
        unitOfWorkMock.SetupGet(uow => uow.Users).Returns(userRepositoryMock.Object);
        unitOfWorkMock.Setup(uow => uow.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var passwordHasherMock = new Mock<IPasswordHasher>();
        configurePasswordHasher(passwordHasherMock);

        var userServiceMock = new Mock<IUserService>();
        configureUserService(userServiceMock);

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddScoped<RegisterUserUseCase>();
        builder.Services.AddSingleton(unitOfWorkMock.Object);
        builder.Services.AddSingleton(passwordHasherMock.Object);
        builder.Services.AddSingleton(userServiceMock.Object);

        var app = builder.Build();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.MapUsersEndpoints();
        return app;
    }
}
