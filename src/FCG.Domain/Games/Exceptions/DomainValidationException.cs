namespace FCG.Domain.Games.Exceptions;

public class DomainValidationException : DomainException
{
    public DomainValidationException(string message) : base(message) { }
}
