namespace FCG.Domain.Games.Events;

public record GameCreatedEvent(Guid GameId, string Title, DateTime OccurredAt);
