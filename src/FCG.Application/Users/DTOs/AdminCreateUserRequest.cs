namespace FCG.Application.Users.DTOs;

public record AdminCreateUserRequest(
    string Name,
    string Email,
    string Password,
    Guid RoleId);