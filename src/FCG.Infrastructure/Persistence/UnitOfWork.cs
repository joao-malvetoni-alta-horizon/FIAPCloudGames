using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence.Context;

namespace FCG.Infrastructure.Persistence;

public class UnitOfWork(
    AppDbContext context,
    IUserRepository users,
    IRoleRepository roles,
    IUserOwnedGameRepository userOwnedGames) : IUserUnitOfWork
{
    public IUserRepository Users { get; } = users;
    public IRoleRepository Roles { get; } = roles;
    public IUserOwnedGameRepository UserOwnedGames { get; } = userOwnedGames;

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);
}
