using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Interfaces;
using FCG.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence.Repositories;

public class GameRepository(AppDbContext context) : IGameRepository
{
    public async Task<Game?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Games.FirstOrDefaultAsync(g => g.Id == id, ct);
    }

    public async Task<(IReadOnlyList<Game> Items, int TotalCount)> ListAsync(
        int page, int pageSize, GameGenre? genre = null, CancellationToken ct = default)
    {
        var query = context.Games.AsQueryable();

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

    public async Task AddAsync(Game game, CancellationToken ct = default)
    {
        await context.Games.AddAsync(game, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Game game, CancellationToken ct = default)
    {
        context.Games.Update(game);
        await context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByTitleAsync(string title, CancellationToken ct = default)
    {
        return await context.Games.AnyAsync(g => g.Title.Value == title, ct);
    }
}
