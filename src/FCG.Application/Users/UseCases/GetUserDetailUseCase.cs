using FCG.Application.Users.DTOs;
using FCG.Application.Users.Interfaces;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;

namespace FCG.Application.Users.UseCases;

public class GetUserDetailUseCase(IUserRepository userRepository) : IGetUserDetailUseCase
{
    public async Task<UserDetailResponse> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetWithOwnedGamesAsync(userId, cancellationToken)
                   ?? throw new UserNotFoundException(userId);

        var ownedGames = user.OwnedGames
            .OrderByDescending(g => g.AcquiredAt)
            .Select(g => new UserOwnedGameResponse(g.Id, g.UserId, g.GameId, g.PricePaid, g.AcquiredAt))
            .ToList();

        return new UserDetailResponse(user.Id, user.Name.Value, user.Email.Address, user.Role?.Name ?? string.Empty, user.IsActive, ownedGames);
    }
}