using FCG.Domain.Shared;
using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence.Context;

namespace FCG.Infrastructure.Persistence;

public class UnitOfWork(
    AppDbContext context,
    IUserRepository users,
    IRoleRepository roles,
    IUserGameLibraryRepository userGameLibraries) : IUserUnitOfWork
{
    public IUserRepository Users { get; } = users;
    public IRoleRepository Roles { get; } = roles;
    public IUserGameLibraryRepository UserGameLibraries { get; } = userGameLibraries;

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);
}
