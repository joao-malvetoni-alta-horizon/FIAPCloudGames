using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Tests.Infrastructure.Persistence;

public class DatabaseSeederTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task SeedAsync_WhenNoAdminExists_ShouldCreateAdminUser()
    {
        await using var db = CreateInMemoryContext();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        passwordHasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-password");
        var loggerMock = new Mock<ILogger<DatabaseSeeder>>();

        var seeder = new DatabaseSeeder(db, passwordHasherMock.Object, loggerMock.Object);
        await seeder.SeedAsync();

        var adminRoleId = RoleType.Administrator.ToRoleId();
        var admin = await db.Users.FirstOrDefaultAsync(u => u.RoleId == adminRoleId);
        admin.Should().NotBeNull();
        admin!.Email.Address.Should().Be("admin@fcg.com");
        admin.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task SeedAsync_WhenAdminAlreadyExists_ShouldNotCreateAnotherAdmin()
    {
        await using var db = CreateInMemoryContext();
        var adminRoleId = RoleType.Administrator.ToRoleId();
        var existingAdmin = User.Create("Existing Admin", "admin@fcg.com", "$2a$12$somehash", adminRoleId);
        db.Users.Add(existingAdmin);
        await db.SaveChangesAsync();

        var passwordHasherMock = new Mock<IPasswordHasher>();
        var loggerMock = new Mock<ILogger<DatabaseSeeder>>();

        var seeder = new DatabaseSeeder(db, passwordHasherMock.Object, loggerMock.Object);
        await seeder.SeedAsync();

        var adminCount = await db.Users.CountAsync(u => u.RoleId == adminRoleId);
        adminCount.Should().Be(1);
        passwordHasherMock.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SeedAsync_WhenNoAdminExists_ShouldHashPasswordBeforeSaving()
    {
        await using var db = CreateInMemoryContext();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        passwordHasherMock.Setup(h => h.Hash("Admin@123")).Returns("hashed-admin-password");
        var loggerMock = new Mock<ILogger<DatabaseSeeder>>();

        var seeder = new DatabaseSeeder(db, passwordHasherMock.Object, loggerMock.Object);
        await seeder.SeedAsync();

        passwordHasherMock.Verify(h => h.Hash("Admin@123"), Times.Once);
        var admin = await db.Users.FirstAsync(u => u.RoleId == RoleType.Administrator.ToRoleId());
        admin.PasswordHash.Should().Be("hashed-admin-password");
    }
}