using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Users.UseCases;

public class RegisterUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenEmailAlreadyExists_ShouldThrowUserAlreadyExistsException()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUserUnitOfWork>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock
            .Setup(service => service.CheckEmailUniquenessAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserAlreadyExistsException("existing@example.com"));

        var useCase = new RegisterUserUseCase(unitOfWorkMock.Object, passwordHasherMock.Object, userServiceMock.Object);
        var request = new RegisterUserRequest("Existing User", "existing@example.com", "Valid@123");

        // Act
        var act = async () => await useCase.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UserAlreadyExistsException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordIsInvalid_ShouldThrowUserDomainException()
    {
        // Arrange
        var usersRepositoryMock = new Mock<IUserRepository>();
        var unitOfWorkMock = new Mock<IUserUnitOfWork>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var userServiceMock = new Mock<IUserService>();
        unitOfWorkMock.SetupGet(uow => uow.Users).Returns(usersRepositoryMock.Object);

        var useCase = new RegisterUserUseCase(unitOfWorkMock.Object, passwordHasherMock.Object, userServiceMock.Object);
        var request = new RegisterUserRequest("Weak Password User", "weak@example.com", "abc");

        // Act
        var act = async () => await useCase.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UserDomainException>();
        passwordHasherMock.Verify(hasher => hasher.Hash(It.IsAny<string>()), Times.Never);
        usersRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRequestIsValid_ShouldAddUserAndCommit()
    {
        // Arrange
        var usersRepositoryMock = new Mock<IUserRepository>();
        var unitOfWorkMock = new Mock<IUserUnitOfWork>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var userServiceMock = new Mock<IUserService>();
        unitOfWorkMock.SetupGet(uow => uow.Users).Returns(usersRepositoryMock.Object);
        passwordHasherMock.Setup(hasher => hasher.Hash("Strong@123")).Returns("hashed-password");

        var useCase = new RegisterUserUseCase(unitOfWorkMock.Object, passwordHasherMock.Object, userServiceMock.Object);
        var request = new RegisterUserRequest("New User", "new@example.com", "Strong@123");

        // Act
        var response = await useCase.ExecuteAsync(request, CancellationToken.None);

        // Assert
        response.Email.Should().Be("new@example.com");
        response.Name.Should().Be("New User");
        response.RoleId.Should().Be(RoleType.User.ToRoleId());
        usersRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
