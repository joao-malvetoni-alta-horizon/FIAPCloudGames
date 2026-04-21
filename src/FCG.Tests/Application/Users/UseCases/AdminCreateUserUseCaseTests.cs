using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Users.UseCases;

public class AdminCreateUserUseCaseTests
{
    private readonly Mock<IUserUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IRoleRepository> _roleRepoMock = new();

    private AdminCreateUserUseCase CreateUseCase()
    {
        _unitOfWorkMock.SetupGet(uow => uow.Users).Returns(_userRepoMock.Object);
        _unitOfWorkMock.SetupGet(uow => uow.Roles).Returns(_roleRepoMock.Object);
        return new AdminCreateUserUseCase(_unitOfWorkMock.Object, _passwordHasherMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailAlreadyExists_ShouldThrowUserAlreadyExistsException()
    {
        _userServiceMock
            .Setup(s => s.CheckEmailUniquenessAsync("dup@example.com", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserAlreadyExistsException("dup@example.com"));

        var useCase = CreateUseCase();
        var request = new AdminCreateUserRequest("Admin", "dup@example.com", "Valid@123", RoleType.Administrator.ToRoleId());

        var act = async () => await useCase.ExecuteAsync(request);

        await act.Should().ThrowAsync<UserAlreadyExistsException>();
        _roleRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRoleNotFound_ShouldThrowUserDomainException()
    {
        var unknownRoleId = Guid.NewGuid();
        _roleRepoMock
            .Setup(r => r.GetByIdAsync(unknownRoleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        var useCase = CreateUseCase();
        var request = new AdminCreateUserRequest("Admin", "admin@example.com", "Valid@123", unknownRoleId);

        var act = async () => await useCase.ExecuteAsync(request);

        await act.Should().ThrowAsync<UserDomainException>()
            .WithMessage($"*{unknownRoleId}*");
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordIsWeak_ShouldThrowUserDomainException()
    {
        var roleId = RoleType.Administrator.ToRoleId();
        var role = Role.Create("Administrador");
        _roleRepoMock
            .Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var useCase = CreateUseCase();
        var request = new AdminCreateUserRequest("Admin", "admin@example.com", "weak", roleId);

        var act = async () => await useCase.ExecuteAsync(request);

        await act.Should().ThrowAsync<UserDomainException>();
        _passwordHasherMock.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRequestIsValid_ShouldCreateUserAndCommit()
    {
        var roleId = RoleType.Administrator.ToRoleId();
        var role = Role.Create("Administrador");
        _roleRepoMock
            .Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _passwordHasherMock
            .Setup(h => h.Hash("Valid@123"))
            .Returns("hashed-value");

        var useCase = CreateUseCase();
        var request = new AdminCreateUserRequest("New Admin", "newadmin@example.com", "Valid@123", roleId);

        var response = await useCase.ExecuteAsync(request);

        response.Should().NotBeNull();
        response.Name.Should().Be("New Admin");
        response.Email.Should().Be("newadmin@example.com");
        response.RoleId.Should().Be(role.Id);
        response.Id.Should().NotBeEmpty();
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}