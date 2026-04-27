namespace FCG.Application.Users.DTOs;

/// <summary>
/// Request para login de usuário
/// </summary>
public record LoginRequest(
    string Email,
    string Password);

/// <summary>
/// Response para login de usuário
/// </summary>
public record LoginResponse(
    string Token,
    string UserName,
    string Email,
    DateTime ExpiresAt);

/// <summary>
/// Request para registro de usuário
/// </summary>
public record RegisterRequest(
    string UserName,
    string Email,
    string Password);

/// <summary>
/// Response para registro de usuário
/// </summary>
public record RegisterResponse(
    Guid UserId,
    string UserName,
    string Email,
    DateTime CreatedAt);