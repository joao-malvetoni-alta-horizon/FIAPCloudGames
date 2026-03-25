namespace FCG.Domain.Users.Interfaces;

/// <summary>
/// Unit of Work — agrupa todos os repositórios do domínio de usuário e persiste as alterações em uma única transação.
/// </summary>
public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IUserGameLibraryRepository UserGameLibraries { get; }

    /// <summary>Persiste todas as alterações pendentes no banco de dados.</summary>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
