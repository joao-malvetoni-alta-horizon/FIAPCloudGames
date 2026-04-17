using FCG.Domain.Users.Entities;
using FCG.Domain.Shared;

namespace FCG.Domain.Users.Interfaces;

/// <summary>Contrato de repositório para o agregado <see cref="UserOwnedGame"/>.</summary>
public interface IUserOwnedGameRepository : IRepository<UserOwnedGame>
{
    /// <summary>Retorna todas as entradas de biblioteca de um determinado usuário.</summary>
    Task<IReadOnlyList<UserOwnedGame>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Retorna <c>true</c> se o usuário já possuir o jogo especificado.</summary>
    Task<bool> ExistsAsync(Guid userId, Guid gameId, CancellationToken cancellationToken = default);
}
