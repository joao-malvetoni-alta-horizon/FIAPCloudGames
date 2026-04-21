namespace FCG.Domain.Games.Exceptions;

public class OverlappingPromotionException()
    : DomainException("The game already has an active promotion overlapping this period.");