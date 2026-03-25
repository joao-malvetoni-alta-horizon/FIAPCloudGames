using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;

namespace FCG.Domain.Games.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Game> Items, int TotalCount)> ListAsync(int page, int pageSize, GameGenre? genre = null, CancellationToken ct = default);
    Task AddAsync(Game game, CancellationToken ct = default);
    Task UpdateAsync(Game game, CancellationToken ct = default);
    Task<bool> ExistsByTitleAsync(string title, CancellationToken ct = default);
}
