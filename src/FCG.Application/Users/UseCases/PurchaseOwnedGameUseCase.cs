using FCG.Application.Users.DTOs;
using FCG.Application.Users.Interfaces;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;

namespace FCG.Application.Users.UseCases;

public class PurchaseOwnedGameUseCase(
    IUserUnitOfWork unitOfWork,
    IGameRepository gameRepository) : IPurchaseOwnedGameUseCase
{
    public async Task<PurchaseOwnedGameResponse> ExecuteAsync(
        Guid userId,
        PurchaseOwnedGameRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
                   ?? throw new UserNotFoundException(userId);

        if (!user.IsActive)
            throw new UserDomainException("Inactive users cannot acquire games.");

        var game = await gameRepository.GetByIdAsync(request.GameId, cancellationToken)
                   ?? throw new GameNotFoundException(request.GameId);

        if (game.Status != GameStatus.Active)
            throw new DomainValidationException("Only active games can be acquired.");

        var alreadyOwned = await unitOfWork.UserOwnedGames.ExistsAsync(userId, request.GameId, cancellationToken);
        if (alreadyOwned)
            throw new UserAlreadyOwnsGameException(userId, request.GameId);

        var acquiredGame = UserOwnedGame.Create(userId, request.GameId, game.Price.Amount);

        await unitOfWork.UserOwnedGames.AddAsync(acquiredGame, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return new PurchaseOwnedGameResponse(
            acquiredGame.Id,
            acquiredGame.UserId,
            acquiredGame.GameId,
            acquiredGame.PricePaid,
            acquiredGame.AcquiredAt);
    }
}
