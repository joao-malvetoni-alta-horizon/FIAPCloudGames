namespace FCG.Application.Users.DTOs;

public record RegisterUserResponse(
    Guid Id,
    string Name,
    string Email,
    Guid RoleId);
