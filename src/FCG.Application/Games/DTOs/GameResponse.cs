using FCG.Domain.Games.Enums;

namespace FCG.Application.Games.DTOs;

public record GameResponse(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    GameGenre Genre,
    GameStatus Status,
    DateOnly ReleaseDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
