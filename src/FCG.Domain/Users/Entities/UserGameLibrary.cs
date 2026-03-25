using FCG.Domain.Games.Entities;
using FCG.Domain.Shared;
using FCG.Domain.Users.Exceptions;

namespace FCG.Domain.Users.Entities;

/// <summary>Representa uma entrada na biblioteca de jogos de um usuário — um jogo que ele adquiriu.</summary>
public class UserGameLibrary : Entity
{
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public DateTime AcquiredAt { get; private set; }
    public decimal PricePaid { get; private set; }

    public User? User { get; private set; }
    public Game? Game { get; private set; }

    /// <summary>Exigido pelo EF Core.</summary>
    protected UserGameLibrary() { }

    private UserGameLibrary(Guid userId, Guid gameId, decimal pricePaid) : base()
    {
        UserId = userId;
        GameId = gameId;
        PricePaid = pricePaid;
        AcquiredAt = DateTime.UtcNow;
    }

    /// <summary>Cria uma nova entrada na biblioteca.</summary>
    /// <param name="userId">Id do usuário que está adquirindo o jogo.</param>
    /// <param name="gameId">Id do jogo adquirido.</param>
    /// <param name="pricePaid">Preço pago no momento da aquisição (>= 0).</param>
    /// <exception cref="UserDomainException">Lançada em caso de entrada inválida.</exception>
    public static UserGameLibrary Create(Guid userId, Guid gameId, decimal pricePaid)
    {
        if (userId == Guid.Empty)
            throw new UserDomainException("UserId cannot be an empty Guid.");
        if (gameId == Guid.Empty)
            throw new UserDomainException("GameId cannot be an empty Guid.");
        if (pricePaid < 0)
            throw new UserDomainException("PricePaid cannot be negative.");

        return new UserGameLibrary(userId, gameId, pricePaid);
    }
}
