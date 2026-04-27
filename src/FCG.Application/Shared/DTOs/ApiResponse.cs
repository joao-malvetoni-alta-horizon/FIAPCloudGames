using FCG.Domain.Shared;

namespace FCG.Application.Shared.DTOs;

/// <summary>
/// DTO base para respostas padronizadas da API
/// </summary>
public record ApiResponse<T>(
    ApiResponseCode Code,
    string Message,
    T? Data = default,
    IEnumerable<string>? Errors = null)
{
    /// <summary>
    /// Cria uma resposta de sucesso
    /// </summary>
    public static ApiResponse<T> Success(T data, string message = "Operação realizada com sucesso")
        => new(ApiResponseCode.Success, message, data);

    /// <summary>
    /// Cria uma resposta de erro
    /// </summary>
    public static ApiResponse<T> Error(ApiResponseCode code, string message, IEnumerable<string>? errors = null)
        => new(code, message, default, errors);

    /// <summary>
    /// Cria uma resposta de erro genérico
    /// </summary>
    public static ApiResponse<T> Error(string message, IEnumerable<string>? errors = null)
        => new(ApiResponseCode.InternalServerError, message, default, errors);

    /// <summary>
    /// Cria uma resposta de não encontrado
    /// </summary>
    public static ApiResponse<T> NotFound(string message = "Recurso não encontrado")
        => new(ApiResponseCode.NotFound, message);

    /// <summary>
    /// Cria uma resposta de dados inválidos
    /// </summary>
    public static ApiResponse<T> BadRequest(string message, IEnumerable<string>? errors = null)
        => new(ApiResponseCode.BadRequest, message, default, errors);
}