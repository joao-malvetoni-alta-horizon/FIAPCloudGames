using FCG.Application.Shared.DTOs;
using FCG.Application.Users.DTOs;
using FCG.Application.Users.Interfaces;
using FCG.Domain.Shared;

namespace FCG.API.Endpoints;

/// <summary>
/// Endpoints para autenticação de usuários
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Realiza login do usuário")
            .WithDescription("Autentica um usuário e retorna um token JWT")
            .Produces<ApiResponse<LoginResponse>>(200)
            .Produces<ApiResponse<object>>(400)
            .Produces<ApiResponse<object>>(401);

        group.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .WithSummary("Registra um novo usuário")
            .WithDescription("Cria uma nova conta de usuário")
            .Produces<ApiResponse<RegisterResponse>>(201)
            .Produces<ApiResponse<object>>(400)
            .Produces<ApiResponse<object>>(409);
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        ILoginUseCase useCase,
        CancellationToken ct)
    {
        try
        {
            var response = await useCase.ExecuteAsync(request, ct);
            return Results.Ok(ApiResponse<LoginResponse>.Success(response, "Login realizado com sucesso"));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ApiResponse<LoginResponse>.Error(
                ApiResponseCode.Unauthorized,
                "Credenciais inválidas",
                new[] { ex.Message }));
        }
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        IRegisterUseCase useCase,
        CancellationToken ct)
    {
        try
        {
            var response = await useCase.ExecuteAsync(request, ct);
            return Results.Created($"/api/auth/users/{response.UserId}",
                ApiResponse<RegisterResponse>.Success(response, "Usuário registrado com sucesso"));
        }
        catch (Exception ex)
        {
            var code = ex.Message.Contains("Email") ? ApiResponseCode.Conflict :
                      ex.Message.Contains("UserName") ? ApiResponseCode.Conflict :
                      ApiResponseCode.BadRequest;

            return Results.BadRequest(ApiResponse<RegisterResponse>.Error(
                code,
                "Erro ao registrar usuário",
                new[] { ex.Message }));
        }
    }
}