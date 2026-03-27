using FCG.Domain.Games.Entities;
using FCG.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserGameLibrary> UserGameLibrary => Set<UserGameLibrary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Shared.Entity>()
            .Where(e => e.State == EntityState.Modified))
        {
            entry.Property(nameof(Domain.Shared.Entity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
