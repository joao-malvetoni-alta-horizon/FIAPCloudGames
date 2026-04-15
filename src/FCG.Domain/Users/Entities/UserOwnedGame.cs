using FCG.Domain.Games.Entities;
using FCG.Domain.Shared;
using FCG.Domain.Users.Exceptions;

namespace FCG.Domain.Users.Entities;

public class UserOwnedGame : Entity
{
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public DateTime AcquiredAt { get; private set; }
    public decimal PricePaid { get; private set; }

    public User? User { get; private set; }
    public Game? Game { get; private set; }
    protected UserOwnedGame() { }

    private UserOwnedGame(Guid userId, Guid gameId, decimal pricePaid) : base()
    {
        UserId = userId;
        GameId = gameId;
        PricePaid = pricePaid;
        AcquiredAt = DateTime.UtcNow;
    }

    public static UserOwnedGame Create(Guid userId, Guid gameId, decimal pricePaid)
    {
        if (userId == Guid.Empty)
            throw new UserDomainException("UserId cannot be an empty Guid.");
        if (gameId == Guid.Empty)
            throw new UserDomainException("GameId cannot be an empty Guid.");
        if (pricePaid < 0)
            throw new UserDomainException("PricePaid cannot be negative.");

        return new UserOwnedGame(userId, gameId, pricePaid);
    }
}
