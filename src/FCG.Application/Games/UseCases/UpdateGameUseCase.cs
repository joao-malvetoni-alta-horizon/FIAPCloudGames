using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Application.Games.Mappers;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Shared;

namespace FCG.Application.Games.UseCases;

public class UpdateGameUseCase(IGameRepository repository, IUnitOfWork unitOfWork) : IUpdateGameUseCase
{
    public async Task<GameResponse> ExecuteAsync(Guid id, UpdateGameRequest request, CancellationToken ct = default)
    {
        var game = await repository.GetByIdAsync(id, ct)
                   ?? throw new GameNotFoundException(id);

        game.Update(
            request.Title,
            request.Description,
            request.Price,
            request.Genre,
            request.ReleaseDate,
            request.Status);

        repository.Update(game);
        await unitOfWork.CommitAsync(ct);

        return GameMapper.ToResponse(game);
    }
}