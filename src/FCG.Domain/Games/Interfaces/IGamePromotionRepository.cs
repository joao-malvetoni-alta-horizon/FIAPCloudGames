using FCG.Domain.Games.Entities;
using FCG.Domain.Shared;

namespace FCG.Domain.Games.Interfaces;

public interface IGamePromotionRepository : IRepository<GamePromotion>
{
    Task<IReadOnlyList<GamePromotion>> GetByGameIdAsync(Guid gameId, CancellationToken ct = default);

    Task<bool> HasOverlappingActivePromotionAsync(
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        Guid? excludeId = null,
        CancellationToken ct = default);
}