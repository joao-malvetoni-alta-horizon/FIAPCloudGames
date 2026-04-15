using FCG.Application.Users.DTOs;
using FCG.Application.Users.Interfaces;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;

namespace FCG.Application.Users.UseCases;

public class GetUserOwnedGamesUseCase(IUserUnitOfWork unitOfWork) : IGetUserOwnedGamesUseCase
{
    public async Task<IReadOnlyList<UserOwnedGameResponse>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
                   ?? throw new UserNotFoundException(userId);

        var libraryEntries = await unitOfWork.UserGameLibraries.GetByUserIdAsync(user.Id, cancellationToken);

        return libraryEntries
            .OrderByDescending(entry => entry.AcquiredAt)
            .Select(entry => new UserOwnedGameResponse(
                entry.Id,
                entry.UserId,
                entry.GameId,
                entry.PricePaid,
                entry.AcquiredAt))
            .ToList();
    }
}
