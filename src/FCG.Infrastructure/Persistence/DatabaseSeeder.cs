using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FCG.Infrastructure.Persistence;

public class DatabaseSeeder(AppDbContext db, IPasswordHasher passwordHasher, ILogger<DatabaseSeeder> logger)
{
    private const string AdminEmail = "admin@fcg.com";
    private const string AdminPassword = "Admin@123";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var adminRoleId = RoleType.Administrator.ToRoleId();
        var adminExists = await db.Users.AnyAsync(u => u.RoleId == adminRoleId, cancellationToken);

        if (adminExists) return;

        var passwordHash = passwordHasher.Hash(AdminPassword);
        var admin = User.Create("Administrador", AdminEmail, passwordHash, adminRoleId);

        db.Users.Add(admin);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Admin criado — email: {Email} | senha: {Password}", AdminEmail, AdminPassword);
    }
}