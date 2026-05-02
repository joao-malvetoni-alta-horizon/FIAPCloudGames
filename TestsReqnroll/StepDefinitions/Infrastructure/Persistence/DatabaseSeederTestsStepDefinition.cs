using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Interfaces;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
public class DatabaseSeederTestsStepDefinition
{
    private readonly AppDbContext _db;
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ILogger<DatabaseSeeder>> _loggerMock = new();
    private readonly DatabaseSeeder _seeder;

    public DatabaseSeederTestsStepDefinition()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _seeder = new DatabaseSeeder(_db, _passwordHasherMock.Object, _loggerMock.Object);
    }

    [Given(@"que não existe nenhum usuário administrador no sistema")]
    public async Task GivenNaoExisteAdmin()
    {
        var adminRoleId = RoleType.Administrator.ToRoleId();
        var admins = await _db.Users.Where(u => u.RoleId == adminRoleId).ToListAsync();
        admins.Should().BeEmpty();
    }

    [Given(@"que já existe um administrador com o e-mail ""(.*)"" no sistema")]
    public async Task GivenAdminJaExiste(string email)
    {
        var adminRoleId = RoleType.Administrator.ToRoleId();
        var existingAdmin = User.CreateRootAdmin(
            "Existing Admin",
            email,
            "$2a$12$somehash",
            adminRoleId);

        _db.Users.Add(existingAdmin);
        await _db.SaveChangesAsync();
    }

    [When(@"eu executar a semeadura do banco de dados")]
    public async Task WhenExecutarSeeder()
    {
        _passwordHasherMock.Setup(h => h.Hash("Admin@123")).Returns("hashed-admin-password");
        await _seeder.SeedAsync();
    }

    [Then(@"um usuário com e-mail ""(.*)"" deve ser criado")]
    public async Task ThenValidarAdminCriado(string email)
    {
        var admin = await _db.Users.FirstOrDefaultAsync(u => u.Email.Address == email);
        admin.Should().NotBeNull();
        admin!.IsActive.Should().BeTrue();
    }

    [Then(@"a senha deve ser criptografada antes de ser salva")]
    public async Task ThenValidarHash()
    {
        var admin = await _db.Users.FirstAsync(u => u.RoleId == RoleType.Administrator.ToRoleId());
        admin.PasswordHash.Should().Be("hashed-admin-password");
        _passwordHasherMock.Verify(h => h.Hash("Admin@123"), Times.Once);
    }

    [Then(@"o usuário deve possuir o cargo de ""(.*)""")]
    public async Task ThenValidarCargo(RoleType role)
    {
        var admin = await _db.Users.FirstOrDefaultAsync(u => u.RoleId == role.ToRoleId());
        admin.Should().NotBeNull();
    }

    [Then(@"o sistema não deve criar um novo administrador")]
    public async Task ThenNaoCriarNovo()
    {
        var adminRoleId = RoleType.Administrator.ToRoleId();
        var adminCount = await _db.Users.CountAsync(u => u.RoleId == adminRoleId);
        adminCount.Should().Be(1);
    }

    [Then(@"o processo de criptografia de senha não deve ser acionado")]
    public void ThenHashNaoAcionado()
    {
        _passwordHasherMock.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
    }
}