using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Interfaces;
using FCG.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence.Repositories;

public class GameRepository(AppDbContext context) : RepositoryBase<Game>(context), IGameRepository
{
    public async Task<(IReadOnlyList<Game> Items, int TotalCount)> ListAsync(
        int page, int pageSize, GameGenre? genre = null, CancellationToken ct = default)
    {
        var query = DbSet.AsQueryable();

        if (genre.HasValue)
            query = query.Where(g => g.Genre == genre.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(g => g.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByTitleAsync(string title, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(g => g.Title.Value == title, ct);
    }
}
