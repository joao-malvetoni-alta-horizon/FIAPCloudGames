namespace FCG.Domain.Games.Exceptions;

public class InvalidReleaseDateException : DomainException
{
    public InvalidReleaseDateException()
        : base("Release date cannot be in the past.") { }
}
