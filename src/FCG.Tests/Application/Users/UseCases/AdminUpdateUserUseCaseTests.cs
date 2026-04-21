using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Constants;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Users.UseCases;

public class AdminUpdateUserUseCaseTests
{
    private static readonly Guid AdminRoleId = RoleType.Administrator.ToRoleId();
    private static readonly Guid UserRoleId = RoleType.User.ToRoleId();

    private readonly Mock<IUserUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IRoleRepository> _roleRepoMock = new();

    private AdminUpdateUserUseCase CreateUseCase()
    {
        _unitOfWorkMock.SetupGet(uow => uow.Users).Returns(_userRepoMock.Object);
        _unitOfWorkMock.SetupGet(uow => uow.Roles).Returns(_roleRepoMock.Object);
        return new AdminUpdateUserUseCase(_unitOfWorkMock.Object);
    }

    private static User MakeUser()
    {
        return User.Create("Test User", "test@example.com", "hash", UserRoleId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ShouldThrowUserNotFoundException()
    {
        var missingId = Guid.NewGuid();
        _userRepoMock
            .Setup(r => r.GetByIdAsync(missingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(missingId, new AdminUpdateUserRequest(null, null));

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenTargetIsRootAdmin_ShouldThrowRootAdminOperationForbiddenException()
    {
        var rootAdmin = User.CreateRootAdmin("Root", "root@example.com", "hash", AdminRoleId);
        _userRepoMock
            .Setup(r => r.GetByIdAsync(UserSeedConstants.RootAdminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rootAdmin);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(UserSeedConstants.RootAdminId, new AdminUpdateUserRequest(false, null));

        await act.Should().ThrowAsync<RootAdminOperationForbiddenException>();
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRoleIdNotFound_ShouldThrowUserDomainException()
    {
        var user = MakeUser();
        var unknownRoleId = Guid.NewGuid();
        _userRepoMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock
            .Setup(r => r.GetByIdAsync(unknownRoleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(user.Id, new AdminUpdateUserRequest(null, unknownRoleId));

        await act.Should().ThrowAsync<UserDomainException>();
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithIsActiveFalse_ShouldDeactivateUser()
    {
        var user = MakeUser();
        _userRepoMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var useCase = CreateUseCase();
        var response = await useCase.ExecuteAsync(user.Id, new AdminUpdateUserRequest(false, null));

        response.IsActive.Should().BeFalse();
        _userRepoMock.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithIsActiveTrue_ShouldActivateUser()
    {
        var user = MakeUser();
        user.Deactivate();
        _userRepoMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var useCase = CreateUseCase();
        var response = await useCase.ExecuteAsync(user.Id, new AdminUpdateUserRequest(true, null));

        response.IsActive.Should().BeTrue();
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNewRoleId_ShouldChangeUserRole()
    {
        var user = MakeUser();
        var newRole = Role.Create("Administrador");
        _userRepoMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock
            .Setup(r => r.GetByIdAsync(AdminRoleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newRole);

        var useCase = CreateUseCase();
        var response = await useCase.ExecuteAsync(user.Id, new AdminUpdateUserRequest(null, AdminRoleId));

        response.RoleId.Should().Be(newRole.Id);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullFields_ShouldNotChangeUserAndCommit()
    {
        var user = MakeUser();
        _userRepoMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var useCase = CreateUseCase();
        var response = await useCase.ExecuteAsync(user.Id, new AdminUpdateUserRequest(null, null));

        response.IsActive.Should().BeTrue();
        response.RoleId.Should().Be(UserRoleId);
        _roleRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}