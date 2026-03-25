namespace FCG.Domain.Games.Events;

public record GameUpdatedEvent(Guid GameId, string Title, DateTime OccurredAt);
