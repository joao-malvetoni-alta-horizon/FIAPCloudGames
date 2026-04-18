using FCG.Domain.Games.Exceptions;

namespace FCG.Domain.Users.Exceptions;

public sealed class UserNotFoundException : DomainException
{
    public UserNotFoundException(Guid id)
        : base($"User with id '{id}' was not found.")
    {
    }
}
