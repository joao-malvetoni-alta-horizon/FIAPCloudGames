using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FluentAssertions;

namespace FCG.Tests.Domain.Entities;

// NOTA: adaptado para compatibilidade com o código existente.
// A entidade Game do grupo usa construtor público (não factory method).
// Os testes cobrem o mesmo comportamento especificado via construtor existente.
public class GameTests
{
    private static readonly DateOnly FutureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

    [Fact]
    public void Constructor_ValidParams_ShouldCreateGame()
    {
        var game = new Game("Half-Life 3", "A legendary sequel.", 59.99m, GameGenre.Action, FutureDate);

        game.Title.Value.Should().Be("Half-Life 3");
        game.Price.Amount.Should().Be(59.99m);
        game.Genre.Should().Be(GameGenre.Action);
        game.Status.Should().Be(GameStatus.Active);
        game.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_EmptyTitle_ShouldThrowInvalidGameTitleException()
    {
        var act = () => new Game(string.Empty, "desc", 10m, GameGenre.RPG, FutureDate);
        act.Should().Throw<InvalidGameTitleException>();
    }

    [Fact]
    public void Constructor_NegativePrice_ShouldThrowInvalidPriceException()
    {
        var act = () => new Game("Valid Title", "desc", -1m, GameGenre.RPG, FutureDate);
        act.Should().Throw<InvalidPriceException>();
    }

    [Fact]
    public void Constructor_PastReleaseDate_ShouldThrowInvalidReleaseDateException()
    {
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var act = () => new Game("Valid Title", "desc", 10m, GameGenre.RPG, pastDate);
        act.Should().Throw<InvalidReleaseDateException>();
    }

    [Fact]
    public void Constructor_DescriptionExceedsMaxLength_ShouldThrowDomainValidationException()
    {
        var longDesc = new string('x', 2001);
        var act = () => new Game("Valid Title", longDesc, 10m, GameGenre.RPG, FutureDate);
        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Update_ValidPrice_ShouldChangePrice()
    {
        var game = new Game("Valid Title", "desc", 10m, GameGenre.RPG, FutureDate);
        game.Update(price: 49.99m);

        game.Price.Amount.Should().Be(49.99m);
        game.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_NegativePrice_ShouldThrowInvalidPriceException()
    {
        var game = new Game("Valid Title", "desc", 10m, GameGenre.RPG, FutureDate);
        var act = () => game.Update(price: -5m);
        act.Should().Throw<InvalidPriceException>();
    }
}
