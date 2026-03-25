using FCG.Application.Games.Interfaces;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;

namespace FCG.Application.Games.UseCases;

public class DeleteGameUseCase(IGameRepository repository) : IDeleteGameUseCase
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var game = await repository.GetByIdAsync(id, ct)
                   ?? throw new GameNotFoundException(id);

        game.Deactivate();

        await repository.UpdateAsync(game, ct);
    }
}
