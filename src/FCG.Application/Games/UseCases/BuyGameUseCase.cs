using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Shared;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Interfaces;

namespace FCG.Application.Games.UseCases;

/// <summary>
/// Use case para compra de jogos
/// </summary>
public class BuyGameUseCase(
    IGameRepository gameRepository,
    IUserRepository userRepository,
    IUserGameLibraryRepository userGameLibraryRepository,
    IUnitOfWork unitOfWork) : IBuyGameUseCase
{
    public async Task<BuyGameResponse> ExecuteAsync(BuyGameRequest request, Guid userId, CancellationToken ct = default)
    {
        // Buscar jogo
        var game = await gameRepository.GetByIdAsync(request.GameId, ct);
        if (game is null)
            throw new GameNotFoundException(request.GameId);

        // Verificar se jogo está disponível
        if (game.Status != GameStatus.Available)
            throw new GameNotAvailableException(game.Title);

        // Buscar usuário
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            throw new UserNotFoundException(userId);

        // Verificar se usuário já possui o jogo
        var existingPurchase = await userGameLibraryRepository.GetByUserAndGameAsync(userId, request.GameId, ct);
        if (existingPurchase is not null)
            throw new GameAlreadyOwnedException(game.Title);

        // Criar registro de compra
        var userGameLibrary = UserGameLibrary.Create(userId, request.GameId, game.Price);

        await userGameLibraryRepository.AddAsync(userGameLibrary, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new BuyGameResponse(
            userGameLibrary.Id,
            userGameLibrary.UserId,
            userGameLibrary.GameId,
            game.Title,
            userGameLibrary.PricePaid,
            userGameLibrary.PurchasedAt);
    }
}