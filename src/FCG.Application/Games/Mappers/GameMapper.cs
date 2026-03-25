using FCG.Application.Games.DTOs;
using FCG.Domain.Games.Entities;

namespace FCG.Application.Games.Mappers;

internal static class GameMapper
{
    public static GameResponse ToResponse(Game game)
    {
        return new GameResponse(
            game.Id,
            game.Title.Value,
            game.Description,
            game.Price.Amount,
            game.Genre,
            game.Status,
            game.ReleaseDate,
            game.CreatedAt,
            game.UpdatedAt);
    }
}