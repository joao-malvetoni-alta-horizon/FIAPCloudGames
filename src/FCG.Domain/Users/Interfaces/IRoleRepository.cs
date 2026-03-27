using FCG.Domain.Users.Entities;
using FCG.Domain.Shared;

namespace FCG.Domain.Users.Interfaces;

/// <summary>Contrato de repositório para o agregado <see cref="Role"/>.</summary>
public interface IRoleRepository : IRepository<Role>
{
    /// <summary>Busca um perfil pelo nome (sem distinção de maiúsculas/minúsculas).</summary>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
