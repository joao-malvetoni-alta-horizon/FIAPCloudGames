using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence.Repositories;

public class UserGameLibraryRepository(AppDbContext context)
    : RepositoryBase<UserGameLibrary>(context), IUserGameLibraryRepository
{
    public async Task<IReadOnlyList<UserGameLibrary>> GetByUserIdAsync(
        Guid userId, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(ugl => ugl.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(
        Guid userId, Guid gameId, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(
            ugl => ugl.UserId == userId && ugl.GameId == gameId,
            cancellationToken);
}
