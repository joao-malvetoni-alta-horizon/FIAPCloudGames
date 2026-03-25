namespace FCG.Domain.Games.Exceptions;

public class InvalidGameTitleException : DomainException
{
    public InvalidGameTitleException(string reason)
        : base($"Invalid game title: {reason}") { }
}
