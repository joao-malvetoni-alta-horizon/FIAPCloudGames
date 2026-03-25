namespace FCG.Domain.Games.Exceptions;

public class InvalidPriceException : DomainException
{
    public InvalidPriceException(decimal price)
        : base($"Price cannot be negative. Value provided: {price}") { }
}
