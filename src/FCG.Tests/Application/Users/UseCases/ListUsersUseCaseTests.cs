using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Application.Users.UseCases;

public class ListUsersUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();

    private static User MakeUserWithRole(string name, string email)
    {
        var user = User.Create(name, email, "hash", RoleType.User.ToRoleId());
        return user;
    }

    [Fact]
    public async Task ExecuteAsync_WhenPageIsValid_ShouldReturnPagedResponse()
    {
        var users = new List<User>
        {
            MakeUserWithRole("Alice", "alice@example.com"),
            MakeUserWithRole("Bob", "bob@example.com"),
        };
        _userRepoMock
            .Setup(r => r.ListAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<User>)users, 2));

        var useCase = new ListUsersUseCase(_userRepoMock.Object);
        var response = await useCase.ExecuteAsync(1, 10);

        response.TotalCount.Should().Be(2);
        response.Page.Should().Be(1);
        response.PageSize.Should().Be(10);
        response.Items.Should().HaveCount(2);
        response.Items.Select(i => i.Email).Should().Contain("alice@example.com");
    }

    [Theory]
    [InlineData(0, 10, 1, 10)]
    [InlineData(-5, 10, 1, 10)]
    [InlineData(1, 0, 1, 10)]
    [InlineData(1, -3, 1, 10)]
    [InlineData(1, 50, 1, 10)]
    public async Task ExecuteAsync_WhenPageOrSizeIsOutOfRange_ShouldUseDefaults(
        int inputPage, int inputSize, int expectedPage, int expectedSize)
    {
        _userRepoMock
            .Setup(r => r.ListAsync(expectedPage, expectedSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<User>)[], 0));

        var useCase = new ListUsersUseCase(_userRepoMock.Object);
        var response = await useCase.ExecuteAsync(inputPage, inputSize);

        response.Page.Should().Be(expectedPage);
        response.PageSize.Should().Be(expectedSize);
        _userRepoMock.Verify(r => r.ListAsync(expectedPage, expectedSize, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserHasNoRole_ShouldReturnEmptyRoleName()
    {
        var user = User.Create("No Role", "norole@example.com", "hash", RoleType.User.ToRoleId());
        _userRepoMock
            .Setup(r => r.ListAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<User>)[user], 1));

        var useCase = new ListUsersUseCase(_userRepoMock.Object);
        var response = await useCase.ExecuteAsync(1, 10);

        response.Items.Single().Role.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoUsers_ShouldReturnEmptyList()
    {
        _userRepoMock
            .Setup(r => r.ListAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<User>)[], 0));

        var useCase = new ListUsersUseCase(_userRepoMock.Object);
        var response = await useCase.ExecuteAsync(1, 10);

        response.Items.Should().BeEmpty();
        response.TotalCount.Should().Be(0);
    }
}