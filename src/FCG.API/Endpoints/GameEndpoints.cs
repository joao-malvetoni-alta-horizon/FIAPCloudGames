using System.Security.Claims;
using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Domain.Games.Enums;

namespace FCG.API.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games")
            .WithTags("Games")
            .RequireAuthorization();

        group.MapPost("/", CreateGame)
            .WithName("CreateGame")
            .RequireAuthorization("AdminOnly")
            .Produces<GameResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
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
            .RequireAuthorization("AdminOnly")
            .Produces<GameResponse>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", DeleteGame)
            .WithName("DeleteGame")
            .RequireAuthorization("AdminOnly")
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateGame(
        CreateGameRequest request,
        ICreateGameUseCase useCase,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var roleId = ResolveRoleId(httpContext);
        var response = await useCase.ExecuteAsync(request, roleId, ct);
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
        HttpContext httpContext,
        CancellationToken ct)
    {
        var roleId = ResolveRoleId(httpContext);
        var response = await useCase.ExecuteAsync(id, request, roleId, ct);
        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteGame(
        Guid id,
        IDeleteGameUseCase useCase,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var roleId = ResolveRoleId(httpContext);
        await useCase.ExecuteAsync(id, roleId, ct);
        return Results.NoContent();
    }

    private static Guid ResolveRoleId(HttpContext httpContext)
    {
        var roleIdClaim = httpContext.User.FindFirst("roleId")?.Value;
        return Guid.TryParse(roleIdClaim, out var roleId) ? roleId : Guid.Empty;
    }
}