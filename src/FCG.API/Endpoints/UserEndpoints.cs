using FCG.Application.Users.DTOs;
using FCG.Application.Users.Interfaces;

namespace FCG.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapPost("/{userId:guid}/owned-games", PurchaseOwnedGame)
            .WithName("PurchaseOwnedGame")
            .Produces<PurchaseOwnedGameResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        group.MapGet("/{userId:guid}/owned-games", GetOwnedGames)
            .WithName("GetUserOwnedGames")
            .Produces<IReadOnlyList<UserOwnedGameResponse>>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> PurchaseOwnedGame(
        Guid userId,
        PurchaseOwnedGameRequest request,
        IPurchaseOwnedGameUseCase useCase,
        CancellationToken cancellationToken)
    {
        var response = await useCase.ExecuteAsync(userId, request, cancellationToken);
        return Results.Created($"/api/users/{userId}/owned-games/{response.Id}", response);
    }

    private static async Task<IResult> GetOwnedGames(
        Guid userId,
        IGetUserOwnedGamesUseCase useCase,
        CancellationToken cancellationToken)
    {
        var response = await useCase.ExecuteAsync(userId, cancellationToken);
        return Results.Ok(response);
    }
}
