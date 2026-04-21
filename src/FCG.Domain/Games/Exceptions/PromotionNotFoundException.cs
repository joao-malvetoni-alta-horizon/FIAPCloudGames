namespace FCG.Domain.Games.Exceptions;

public class PromotionNotFoundException : DomainException
{
    public PromotionNotFoundException(Guid id)
        : base($"Promotion with id '{id}' was not found.") { }
}