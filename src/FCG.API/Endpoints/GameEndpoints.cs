using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Domain.Games.Enums;

namespace FCG.API.Endpoints;

public static class GameEndpoints
{
    private const string RoleIdHeaderName = "X-Role-Id";

    public static void MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games")
            .WithTags("Games");

        group.MapPost("/", CreateGame)
            .WithName("CreateGame")
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
            .Produces<GameResponse>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", DeleteGame)
            .WithName("DeleteGame")
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
        if (!httpContext.Request.Headers.TryGetValue(RoleIdHeaderName, out var roleIdHeader))
            return Guid.Empty;

        return Guid.TryParse(roleIdHeader, out var roleId)
            ? roleId
            : Guid.Empty;
    }
}
