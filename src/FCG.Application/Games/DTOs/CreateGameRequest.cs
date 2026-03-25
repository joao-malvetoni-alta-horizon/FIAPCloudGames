using FCG.Domain.Games.Enums;

namespace FCG.Application.Games.DTOs;

public record CreateGameRequest(
    string Title,
    string Description,
    decimal Price,
    GameGenre Genre,
    DateOnly ReleaseDate);
