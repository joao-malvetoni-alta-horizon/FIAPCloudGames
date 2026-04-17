using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FCG.Domain.Users.Services;
using FluentAssertions;
using Moq;

namespace FCG.Tests.Domain.Services;

public class UserServiceTests
{
    [Fact]
    public async Task CheckEmailUniquenessAsync_WhenEmailAlreadyExists_ShouldThrowUserAlreadyExistsException()
    {
        // Arrange
        const string email = "existing.user@example.com";
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(repository => repository.ExistsByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new UserService(userRepositoryMock.Object);

        // Act
        var act = async () => await service.CheckEmailUniquenessAsync(email, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UserAlreadyExistsException>()
            .WithMessage($"*{email}*");
    }
}
