using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Shared;

namespace FCG.Domain.Games.Interfaces;

public interface IGameRepository : IRepository<Game>
{
    Task<(IReadOnlyList<Game> Items, int TotalCount)> ListAsync(int page, int pageSize, GameGenre? genre = null, CancellationToken ct = default);
    Task<bool> ExistsByTitleAsync(string title, CancellationToken ct = default);
}
