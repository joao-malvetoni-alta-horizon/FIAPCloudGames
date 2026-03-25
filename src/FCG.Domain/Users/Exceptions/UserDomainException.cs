using FCG.Domain.Games.Exceptions;

namespace FCG.Domain.Users.Exceptions;

/// <summary>Lançada quando uma regra de negócio do domínio de usuário é violada.</summary>
public class UserDomainException : DomainException
{
    public UserDomainException(string message) : base(message) { }
}
