using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Domain.Games.Enums;

namespace FCG.API.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games")
            .WithTags("Games");

        group.MapPost("/", CreateGame)
            .WithName("CreateGame")
            .Produces<GameResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetGameById)
            .WithName("GetGameById")
            .Produces<GameResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ListGames)
            .WithName("ListGames")
            .Produces<PagedGameResponse>();

        group.MapPut("/{id:guid}", UpdateGame)
            .WithName("UpdateGame")
            .Produces<GameResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", DeleteGame)
            .WithName("DeleteGame")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateGame(
        CreateGameRequest request,
        ICreateGameUseCase useCase,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);
        return Results.Created($"/api/games/{response.Id}", response);
    }

    private static async Task<IResult> GetGameById(
        Guid id,
        IGetGameUseCase useCase,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(id, ct);
        return Results.Ok(response);
    }

    private static async Task<IResult> ListGames(
        IListGamesUseCase useCase,
        int page = 1,
        int pageSize = 10,
        GameGenre? genre = null,
        CancellationToken ct = default)
    {
        var response = await useCase.ExecuteAsync(page, pageSize, genre, ct);
        return Results.Ok(response);
    }

    private static async Task<IResult> UpdateGame(
        Guid id,
        UpdateGameRequest request,
        IUpdateGameUseCase useCase,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(id, request, ct);
        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteGame(
        Guid id,
        IDeleteGameUseCase useCase,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return Results.NoContent();
    }
}
