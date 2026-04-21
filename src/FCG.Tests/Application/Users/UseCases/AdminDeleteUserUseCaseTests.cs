using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Constants;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Users.UseCases;

public class AdminDeleteUserUseCaseTests
{
    private readonly Mock<IUserUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();

    private AdminDeleteUserUseCase CreateUseCase()
    {
        _unitOfWorkMock.SetupGet(uow => uow.Users).Returns(_userRepoMock.Object);
        return new AdminDeleteUserUseCase(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ShouldThrowUserNotFoundException()
    {
        var missingId = Guid.NewGuid();
        _userRepoMock
            .Setup(r => r.GetByIdAsync(missingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(missingId);

        await act.Should().ThrowAsync<UserNotFoundException>();
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTargetIsRootAdmin_ShouldThrowRootAdminOperationForbiddenException()
    {
        var rootAdmin = User.CreateRootAdmin("Root", "root@example.com", "hash", RoleType.Administrator.ToRoleId());
        _userRepoMock
            .Setup(r => r.GetByIdAsync(UserSeedConstants.RootAdminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rootAdmin);

        var useCase = CreateUseCase();

        var act = async () => await useCase.ExecuteAsync(UserSeedConstants.RootAdminId);

        await act.Should().ThrowAsync<RootAdminOperationForbiddenException>();
        _userRepoMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserExists_ShouldSoftDeleteAndCommit()
    {
        var user = User.Create("Some User", "some@example.com", "hash", RoleType.User.ToRoleId());
        _userRepoMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var useCase = CreateUseCase();
        await useCase.ExecuteAsync(user.Id);

        user.IsActive.Should().BeFalse();
        user.DeletedAt.Should().NotBeNull();
        _userRepoMock.Verify(r => r.Update(user), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}