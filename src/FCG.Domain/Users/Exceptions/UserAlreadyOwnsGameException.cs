using FCG.Domain.Games.Exceptions;

namespace FCG.Domain.Users.Exceptions;

public sealed class UserAlreadyOwnsGameException : DomainException
{
    public UserAlreadyOwnsGameException(Guid userId, Guid gameId)
        : base($"User '{userId}' already owns game '{gameId}'.")
    {
    }
}
