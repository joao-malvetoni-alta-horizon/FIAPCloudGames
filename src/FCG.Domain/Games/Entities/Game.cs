using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Events;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.ValueObjects;

namespace FCG.Domain.Games.Entities;

public class Game
{
    private readonly List<object> _domainEvents = [];

    public Guid Id { get; private set; }
    public GameTitle Title { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public Price Price { get; private set; } = null!;
    public GameGenre Genre { get; private set; }
    public GameStatus Status { get; private set; }
    public DateOnly ReleaseDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    private Game() { }

    public Game(
        string title,
        string description,
        decimal price,
        GameGenre genre,
        DateOnly releaseDate)
    {
        if (releaseDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new InvalidReleaseDateException();

        if (description?.Length > 2000)
            throw new DomainValidationException("Description cannot exceed 2000 characters.");

        Id = Guid.NewGuid();
        Title = new GameTitle(title);
        Description = description ?? string.Empty;
        Price = new Price(price);
        Genre = genre;
        Status = GameStatus.Active;
        ReleaseDate = releaseDate;
        CreatedAt = DateTime.UtcNow;

        _domainEvents.Add(new GameCreatedEvent(Id, Title.Value, CreatedAt));
    }

    public void Update(
        string? title = null,
        string? description = null,
        decimal? price = null,
        GameGenre? genre = null,
        DateOnly? releaseDate = null,
        GameStatus? status = null)
    {
        if (title is not null)
            Title = new GameTitle(title);

        if (description is not null)
        {
            if (description.Length > 2000)
                throw new DomainValidationException("Description cannot exceed 2000 characters.");
            Description = description;
        }

        if (price.HasValue)
            Price = new Price(price.Value);

        if (genre.HasValue)
            Genre = genre.Value;

        if (releaseDate.HasValue)
            ReleaseDate = releaseDate.Value;

        if (status.HasValue)
            Status = status.Value;

        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new GameUpdatedEvent(Id, Title.Value, UpdatedAt.Value));
    }

    public void Deactivate()
    {
        Status = GameStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        _domainEvents.Add(new GameUpdatedEvent(Id, Title.Value, UpdatedAt.Value));
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
