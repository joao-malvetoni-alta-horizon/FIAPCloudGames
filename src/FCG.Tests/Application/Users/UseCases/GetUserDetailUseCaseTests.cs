using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Users.UseCases;

public class GetUserDetailUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ShouldThrowUserNotFoundException()
    {
        var missingId = Guid.NewGuid();
        _userRepoMock
            .Setup(r => r.GetWithOwnedGamesAsync(missingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var useCase = new GetUserDetailUseCase(_userRepoMock.Object);

        var act = async () => await useCase.ExecuteAsync(missingId);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserExists_ShouldReturnDetailResponse()
    {
        var user = User.Create("Detail User", "detail@example.com", "hash", RoleType.User.ToRoleId());
        _userRepoMock
            .Setup(r => r.GetWithOwnedGamesAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var useCase = new GetUserDetailUseCase(_userRepoMock.Object);
        var response = await useCase.ExecuteAsync(user.Id);

        response.Id.Should().Be(user.Id);
        response.Name.Should().Be("Detail User");
        response.Email.Should().Be("detail@example.com");
        response.IsActive.Should().BeTrue();
        response.OwnedGames.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserHasNoRole_ShouldReturnEmptyRoleName()
    {
        var user = User.Create("No Role User", "norole@example.com", "hash", RoleType.User.ToRoleId());
        _userRepoMock
            .Setup(r => r.GetWithOwnedGamesAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var useCase = new GetUserDetailUseCase(_userRepoMock.Object);
        var response = await useCase.ExecuteAsync(user.Id);

        response.Role.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserHasOwnedGames_ShouldReturnGamesOrderedByAcquiredAtDesc()
    {
        var user = User.Create("Gamer", "gamer@example.com", "hash", RoleType.User.ToRoleId());
        var userId = user.Id;

        var game1 = UserOwnedGame.Create(userId, Guid.NewGuid(), 29.99m);
        await Task.Delay(5);
        var game2 = UserOwnedGame.Create(userId, Guid.NewGuid(), 49.99m);

        // Add via reflection since the list is private
        var field = typeof(User).GetField("_ownedGames", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var ownedGames = (List<UserOwnedGame>)field.GetValue(user)!;
        ownedGames.Add(game1);
        ownedGames.Add(game2);

        _userRepoMock
            .Setup(r => r.GetWithOwnedGamesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var useCase = new GetUserDetailUseCase(_userRepoMock.Object);
        var response = await useCase.ExecuteAsync(userId);

        response.OwnedGames.Should().HaveCount(2);
        response.OwnedGames.First().AcquiredAt.Should().BeOnOrAfter(response.OwnedGames.Last().AcquiredAt);
    }
}