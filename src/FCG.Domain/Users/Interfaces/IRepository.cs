using FCG.Domain.Shared;

namespace FCG.Domain.Users.Interfaces;

/// <summary>Abstração assíncrona genérica de repositório para raízes de agregação.</summary>
/// <typeparam name="T">Uma raiz de agregação derivada de <see cref="Entity"/>.</typeparam>
public interface IRepository<T> where T : Entity
{
    /// <summary>Recupera uma entidade pelo seu identificador único.</summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Retorna todas as entidades do tipo <typeparamref name="T"/>.</summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Adiciona uma nova entidade ao repositório.</summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Marca uma entidade como modificada.</summary>
    void Update(T entity);

    /// <summary>Remove uma entidade do repositório.</summary>
    void Delete(T entity);
}
