namespace FCG.Application.Users.DTOs;

public record AdminUpdateUserResponse(
    Guid Id,
    string Name,
    string Email,
    bool IsActive,
    Guid RoleId);