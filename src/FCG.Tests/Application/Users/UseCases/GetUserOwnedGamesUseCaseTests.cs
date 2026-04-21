using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Users.UseCases;

public class GetUserOwnedGamesUseCaseTests
{
    private readonly Mock<IUserUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _usersMock = new();
    private readonly Mock<IUserOwnedGameRepository> _librariesMock = new();
    private readonly GetUserOwnedGamesUseCase _sut;

    public GetUserOwnedGamesUseCaseTests()
    {
        _unitOfWorkMock.SetupGet(u => u.Users).Returns(_usersMock.Object);
        _unitOfWorkMock.SetupGet(u => u.UserOwnedGames).Returns(_librariesMock.Object);
        _sut = new GetUserOwnedGamesUseCase(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowUserNotFoundException()
    {
        var userId = Guid.NewGuid();
        _usersMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var act = async () => await _sut.ExecuteAsync(userId);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserExists_ShouldReturnOwnedGames()
    {
        var userId = Guid.NewGuid();
        var user = CreateActiveUser();
        var first = UserOwnedGame.Create(userId, Guid.NewGuid(), 20m);
        Thread.Sleep(10);
        var second = UserOwnedGame.Create(userId, Guid.NewGuid(), 35m);

        _usersMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _librariesMock.Setup(r => r.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([first, second]);

        var response = await _sut.ExecuteAsync(userId);

        response.Should().HaveCount(2);
        response[0].GameId.Should().Be(second.GameId);
        response[0].PricePaid.Should().Be(35m);
        response[1].GameId.Should().Be(first.GameId);
        response[1].PricePaid.Should().Be(20m);
    }

    [Fact]
    public async Task ExecuteAsync_WhenLibraryIsEmpty_ShouldReturnEmptyList()
    {
        var userId = Guid.NewGuid();
        var user = CreateActiveUser();

        _usersMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _librariesMock.Setup(r => r.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var response = await _sut.ExecuteAsync(userId);

        response.Should().BeEmpty();
    }

    private static User CreateActiveUser()
        => User.Create("John Doe", "john@example.com", "$2a$12$hash", RoleType.User.ToRoleId());
}
