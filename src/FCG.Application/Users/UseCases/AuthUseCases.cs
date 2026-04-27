using FCG.Application.Users.DTOs;
using FCG.Application.Users.Interfaces;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FCG.Domain.Shared;

namespace FCG.Application.Users.UseCases;

/// <summary>
/// Use case para login de usuário
/// </summary>
public class LoginUseCase(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService) : ILoginUseCase
{
    public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null)
            throw new InvalidCredentialsException();

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var token = jwtService.GenerateToken(user);
        var expiresAt = DateTime.Now.AddMinutes(60); // TODO: Pegar do JwtOptions

        return new LoginResponse(token, user.UserName, user.Email, expiresAt);
    }
}

/// <summary>
/// Use case para registro de usuário
/// </summary>
public class RegisterUseCase(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRegisterUseCase
{
    public async Task<RegisterResponse> ExecuteAsync(RegisterRequest request, CancellationToken ct = default)
    {
        // Verificar se email já existe
        var existingUser = await userRepository.GetByEmailAsync(request.Email, ct);
        if (existingUser is not null)
            throw new EmailAlreadyExistsException(request.Email);

        // Verificar se username já existe
        var existingUserName = await userRepository.GetByUserNameAsync(request.UserName, ct);
        if (existingUserName is not null)
            throw new UserNameAlreadyExistsException(request.UserName);

        // Buscar role padrão (User)
        var userRole = await roleRepository.GetByNameAsync("User", ct)
            ?? throw new RoleNotFoundException("User");

        // Criar usuário
        var user = User.Create(request.UserName, request.Email, request.Password, userRole);
        user.SetPasswordHash(passwordHasher.HashPassword(request.Password));

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new RegisterResponse(user.Id, user.UserName, user.Email, user.CreatedAt);
    }
}