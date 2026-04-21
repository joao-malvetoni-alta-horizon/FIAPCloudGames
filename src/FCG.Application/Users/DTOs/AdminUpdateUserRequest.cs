namespace FCG.Application.Users.DTOs;

public record AdminUpdateUserRequest(
    bool? IsActive,
    Guid? RoleId);