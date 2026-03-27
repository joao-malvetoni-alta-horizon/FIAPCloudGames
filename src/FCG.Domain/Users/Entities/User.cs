using FCG.Domain.Shared;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.ValueObjects;

namespace FCG.Domain.Users.Entities;

public class User : Entity
{
    private readonly List<UserGameLibrary> _gameLibrary = [];

    public Name Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    public Guid RoleId { get; private set; }
    public bool IsActive { get; private set; }

    public Role? Role { get; private set; }
    public IReadOnlyCollection<UserGameLibrary> GameLibrary => _gameLibrary.AsReadOnly();

    protected User()
    {
    }

    private User(Name name, Email email, string passwordHash, Guid roleId)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
        IsActive = true;
    }

    public static User Create(string name, string email, string passwordHash, Guid roleId)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new UserDomainException("Password hash cannot be null or empty.");
        if (roleId == Guid.Empty)
            throw new UserDomainException("RoleId cannot be an empty Guid.");

        var emailVo = Email.Create(email);
        var nameVo = Name.Create(name);
        return new User(nameVo, emailVo, passwordHash, roleId);
    }

    public void UpdateName(string name)
    {
        Name = Name.Create(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        Email = Email.Create(email);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newHash)
    {
        if (string.IsNullOrWhiteSpace(newHash))
            throw new UserDomainException("Password hash cannot be null or empty.");

        PasswordHash = newHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new UserDomainException("RoleId cannot be an empty Guid.");

        RoleId = roleId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}