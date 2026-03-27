using FCG.Domain.Games.Exceptions;

namespace FCG.Domain.Users.Exceptions;

public class UserDomainException : DomainException
{
    public UserDomainException(string message) : base(message) { }
}
