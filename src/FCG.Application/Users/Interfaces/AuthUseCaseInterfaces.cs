using FCG.Application.Users.DTOs;

namespace FCG.Application.Users.Interfaces;

/// <summary>
/// Interface para use case de login
/// </summary>
public interface ILoginUseCase
{
    Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken ct = default);
}

/// <summary>
/// Interface para use case de registro
/// </summary>
public interface IRegisterUseCase
{
    Task<RegisterResponse> ExecuteAsync(RegisterRequest request, CancellationToken ct = default);
}