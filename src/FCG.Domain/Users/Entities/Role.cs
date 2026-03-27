using FCG.Domain.Shared;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;

namespace FCG.Domain.Users.Entities;

public class Role : Entity
{
    private readonly List<User> _users = [];

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();
    protected Role() { }

    private Role(string name, string? description) : base()
    {
        Name = name;
        Description = description;
        IsActive = true;
    }
    public static Role Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new UserDomainException("Role name cannot be null or empty.");
        if (name.Length > 50)
            throw new UserDomainException("Role name cannot exceed 50 characters.");
        if (description?.Length > 200)
            throw new UserDomainException("Role description cannot exceed 200 characters.");

        return new Role(name, description);
    }
    internal static Role CreateSeed(Guid id, string name, string? description)
    {
        var role = new Role(name, description);
        role.Id = id;
        return role;
    }
}
