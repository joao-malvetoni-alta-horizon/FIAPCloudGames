namespace FCG.Application.Games.Interfaces;

public interface IDeleteGameUseCase
{
    Task ExecuteAsync(Guid id, CancellationToken ct = default);
}
