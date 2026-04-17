using FCG.Application.Users.DTOs;

namespace FCG.Application.Users.Interfaces;

public interface IGetUserOwnedGamesUseCase
{
    Task<IReadOnlyList<UserOwnedGameResponse>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
