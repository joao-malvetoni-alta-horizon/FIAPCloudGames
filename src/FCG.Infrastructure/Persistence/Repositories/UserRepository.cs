using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : RepositoryBase<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(
            u => u.Email == email,
            cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(
            u => u.Email == email,
            cancellationToken);
}
