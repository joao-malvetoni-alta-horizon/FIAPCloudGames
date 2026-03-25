using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence.Context;
using FCG.Infrastructure.Persistence.Repositories;

namespace FCG.Infrastructure.Persistence;

public class UnitOfWork(
    AppDbContext context,
    IUserRepository users,
    IRoleRepository roles,
    IUserGameLibraryRepository userGameLibraries) : IUnitOfWork
{
    public IUserRepository Users { get; } = users;
    public IRoleRepository Roles { get; } = roles;
    public IUserGameLibraryRepository UserGameLibraries { get; } = userGameLibraries;

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);
}
