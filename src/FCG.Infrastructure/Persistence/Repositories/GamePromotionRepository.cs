using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Interfaces;
using FCG.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence.Repositories;

public class GamePromotionRepository(AppDbContext context)
    : RepositoryBase<GamePromotion>(context), IGamePromotionRepository
{
    public async Task<IReadOnlyList<GamePromotion>> GetByGameIdAsync(Guid gameId, CancellationToken ct = default)
        => await DbSet
            .Where(p => p.GameId == gameId)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync(ct);

    public async Task<bool> HasOverlappingActivePromotionAsync(
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        Guid? excludeId = null,
        CancellationToken ct = default)
    {
        var query = DbSet.Where(p =>
            p.GameId == gameId &&
            p.IsActive &&
            p.StartDate < endDate &&
            p.EndDate > startDate);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(ct);
    }
}