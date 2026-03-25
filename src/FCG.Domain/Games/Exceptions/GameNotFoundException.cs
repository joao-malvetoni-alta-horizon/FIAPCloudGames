namespace FCG.Domain.Games.Exceptions;

public class GameNotFoundException : DomainException
{
    public GameNotFoundException(Guid id)
        : base($"Game with id '{id}' was not found.") { }
}
