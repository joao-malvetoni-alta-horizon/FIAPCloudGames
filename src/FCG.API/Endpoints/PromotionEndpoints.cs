using System.Security.Claims;
using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Domain.Users.Enums;

namespace FCG.API.Endpoints;

public static class PromotionEndpoints
{
    public static void MapPromotionEndpoints(this IEndpointRouteBuilder app)
    {
        var readGroup = app.MapGroup("/api/games/{gameId:guid}/promotions")
            .WithTags("Promotions")
            .RequireAuthorization();

        readGroup.MapGet("/", ListPromotions)
            .WithName("ListPromotions")
            .Produces<IReadOnlyList<PromotionResponse>>();

        readGroup.MapGet("/{id:guid}", GetPromotion)
            .WithName("GetPromotion")
            .Produces<PromotionResponse>()
            .Produces(StatusCodes.Status404NotFound);

        var adminGroup = app.MapGroup("/api/admin/games/{gameId:guid}/promotions")
            .WithTags("Admin - Promotions")
            .RequireAuthorization("AdminOnly");

        adminGroup.MapPost("/", CreatePromotion)
            .WithName("AdminCreatePromotion")
            .Produces<PromotionResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        adminGroup.MapPut("/{id:guid}", UpdatePromotion)
            .WithName("AdminUpdatePromotion")
            .Produces<PromotionResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        adminGroup.MapDelete("/{id:guid}", DeletePromotion)
            .WithName("AdminDeletePromotion")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> ListPromotions(
        Guid gameId,
        IListPromotionsByGameUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(gameId, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPromotion(
        Guid gameId,
        Guid id,
        IGetPromotionUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(id, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreatePromotion(
        Guid gameId,
        CreatePromotionRequest request,
        ICreatePromotionUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var roleId = GetRoleId(user);
        var result = await useCase.ExecuteAsync(gameId, request, roleId, ct);
        return Results.Created($"/api/games/{gameId}/promotions/{result.Id}", result);
    }

    private static async Task<IResult> UpdatePromotion(
        Guid gameId,
        Guid id,
        UpdatePromotionRequest request,
        IUpdatePromotionUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var roleId = GetRoleId(user);
        var result = await useCase.ExecuteAsync(id, request, roleId, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeletePromotion(
        Guid gameId,
        Guid id,
        IDeletePromotionUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var roleId = GetRoleId(user);
        await useCase.ExecuteAsync(id, roleId, ct);
        return Results.NoContent();
    }

    private static Guid GetRoleId(ClaimsPrincipal user)
    {
        var roleIdClaim = user.FindFirstValue("roleId");
        return Guid.TryParse(roleIdClaim, out var roleId) ? roleId : RoleType.User.ToRoleId();
    }
}