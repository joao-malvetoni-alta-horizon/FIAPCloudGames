namespace FCG.Domain.Shared;

/// <summary>
/// Enum para códigos de resposta padronizados da API
/// </summary>
public enum ApiResponseCode
{
    /// <summary>
    /// Operação realizada com sucesso
    /// </summary>
    Success = 200,

    /// <summary>
    /// Recurso criado com sucesso
    /// </summary>
    Created = 201,

    /// <summary>
    /// Recurso não encontrado
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// Dados de entrada inválidos
    /// </summary>
    BadRequest = 400,

    /// <summary>
    /// Acesso não autorizado
    /// </summary>
    Unauthorized = 401,

    /// <summary>
    /// Acesso proibido
    /// </summary>
    Forbidden = 403,

    /// <summary>
    /// Conflito de dados
    /// </summary>
    Conflict = 409,

    /// <summary>
    /// Erro interno do servidor
    /// </summary>
    InternalServerError = 500,

    /// <summary>
    /// Serviço indisponível
    /// </summary>
    ServiceUnavailable = 503
}