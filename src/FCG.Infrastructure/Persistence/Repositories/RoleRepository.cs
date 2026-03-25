using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence.Repositories;

public class RoleRepository(AppDbContext context) : RepositoryBase<Role>(context), IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(
            r => EF.Functions.ILike(r.Name, name),
            cancellationToken);
}
