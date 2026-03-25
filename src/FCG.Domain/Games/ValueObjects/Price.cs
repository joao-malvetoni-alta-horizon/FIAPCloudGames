using FCG.Domain.Games.Exceptions;

namespace FCG.Domain.Games.ValueObjects;

public record Price
{
    public decimal Amount { get; init; }

    public Price(decimal amount)
    {
        if (amount < 0)
            throw new InvalidPriceException(amount);

        Amount = amount;
    }

    private Price() { }
}
