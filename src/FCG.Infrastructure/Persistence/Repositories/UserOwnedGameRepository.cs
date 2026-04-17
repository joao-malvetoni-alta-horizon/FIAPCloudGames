using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence.Repositories;

public class UserOwnedGameRepository(AppDbContext context)
    : RepositoryBase<UserOwnedGame>(context), IUserOwnedGameRepository
{
    public async Task<IReadOnlyList<UserOwnedGame>> GetByUserIdAsync(
        Guid userId, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(ownedGame => ownedGame.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(
        Guid userId, Guid gameId, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(
            ownedGame => ownedGame.UserId == userId && ownedGame.GameId == gameId,
            cancellationToken);
}
