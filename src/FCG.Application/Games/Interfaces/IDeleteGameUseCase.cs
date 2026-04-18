namespace FCG.Application.Games.Interfaces;

public interface IDeleteGameUseCase
{
    Task ExecuteAsync(Guid id, Guid roleId, CancellationToken ct = default);
}
