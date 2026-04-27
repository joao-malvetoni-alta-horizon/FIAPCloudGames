using System.Security.Claims;
using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Application.Shared.DTOs;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Shared;
using FCG.Domain.Users.Exceptions;

namespace FCG.API.Endpoints;

/// <summary>
/// Endpoints para operações de jogos do usuário
/// </summary>
public static class UserGameEndpoints
{
    public static void MapUserGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games")
            .WithTags("User Games")
            .RequireAuthorization();

        group.MapPost("/buy", BuyGameAsync)
            .WithName("BuyGame")
            .WithSummary("Compra um jogo")
            .WithDescription("Permite que um usuário autenticado compre um jogo")
            .Produces<ApiResponse<BuyGameResponse>>(200)
            .Produces<ApiResponse<object>>(400)
            .Produces<ApiResponse<object>>(401)
            .Produces<ApiResponse<object>>(404)
            .Produces<ApiResponse<object>>(409);
    }

    private static async Task<IResult> BuyGameAsync(
        BuyGameRequest request,
        HttpContext httpContext,
        IBuyGameUseCase useCase,
        CancellationToken ct)
    {
        try
        {
            // Extrair userId do JWT token
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)
                ?? httpContext.User.FindFirst("sub");

            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            var response = await useCase.ExecuteAsync(request, userId, ct);
            return Results.Ok(ApiResponse<BuyGameResponse>.Success(response, "Jogo comprado com sucesso"));
        }
        catch (GameNotFoundException ex)
        {
            return Results.NotFound(ApiResponse<BuyGameResponse>.NotFound($"Jogo não encontrado: {ex.Message}"));
        }
        catch (GameNotAvailableException ex)
        {
            return Results.BadRequest(ApiResponse<BuyGameResponse>.BadRequest($"Jogo não disponível: {ex.Message}"));
        }
        catch (GameAlreadyOwnedException ex)
        {
            return Results.Conflict(ApiResponse<BuyGameResponse>.Error(
                ApiResponseCode.Conflict,
                $"Jogo já possui: {ex.Message}"));
        }
        catch (UserNotFoundException ex)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ApiResponse<BuyGameResponse>.Error(
                ApiResponseCode.InternalServerError,
                "Erro interno do servidor",
                new[] { ex.Message }));
        }
    }
}