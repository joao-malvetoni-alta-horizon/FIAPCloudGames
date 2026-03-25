using FCG.Domain.Shared;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.ValueObjects;

namespace FCG.Domain.Users.Entities;

/// <summary>Representa um usuário registrado na plataforma.</summary>
public class User : Entity
{
    private readonly List<UserGameLibrary> _gameLibrary = [];

    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;

    /// <summary>Hash BCrypt da senha do usuário. Nunca armazena texto puro.</summary>
    public string PasswordHash { get; private set; } = null!;

    public Guid RoleId { get; private set; }
    public bool IsActive { get; private set; }

    public Role? Role { get; private set; }
    public IReadOnlyCollection<UserGameLibrary> GameLibrary => _gameLibrary.AsReadOnly();

    /// <summary>Exigido pelo EF Core.</summary>
    protected User() { }

    private User(string name, Email email, string passwordHash, Guid roleId) : base()
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
        IsActive = true;
    }

    /// <summary>
    /// Factory method para criar um novo <see cref="User"/>.
    /// O chamador é responsável por validar a senha em texto puro e fornecer o hash resultante.
    /// </summary>
    /// <param name="name">Nome de exibição do usuário (máx. 150 caracteres).</param>
    /// <param name="email">E-mail bruto — será validado via <see cref="Email.Create"/>.</param>
    /// <param name="passwordHash">Hash BCrypt da senha já validada.</param>
    /// <param name="roleId">Id do perfil a ser atribuído.</param>
    /// <exception cref="UserDomainException">Lançada em caso de falha de validação.</exception>
    public static User Create(string name, string email, string passwordHash, Guid roleId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new UserDomainException("User name cannot be null or empty.");
        if (name.Length > 150)
            throw new UserDomainException("User name cannot exceed 150 characters.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new UserDomainException("Password hash cannot be null or empty.");
        if (roleId == Guid.Empty)
            throw new UserDomainException("RoleId cannot be an empty Guid.");

        var emailVo = Email.Create(email);
        return new User(name.Trim(), emailVo, passwordHash, roleId);
    }

    /// <summary>Atualiza o nome de exibição do usuário.</summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new UserDomainException("User name cannot be null or empty.");
        if (name.Length > 150)
            throw new UserDomainException("User name cannot exceed 150 characters.");

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Atualiza o e-mail do usuário.</summary>
    public void UpdateEmail(string email)
    {
        Email = Email.Create(email);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Substitui o hash de senha armazenado por um novo.</summary>
    public void UpdatePassword(string newHash)
    {
        if (string.IsNullOrWhiteSpace(newHash))
            throw new UserDomainException("Password hash cannot be null or empty.");

        PasswordHash = newHash;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Atribui um perfil diferente ao usuário.</summary>
    public void ChangeRole(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new UserDomainException("RoleId cannot be an empty Guid.");

        RoleId = roleId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Desativa o usuário (soft delete).</summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Reativa um usuário previamente desativado.</summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
