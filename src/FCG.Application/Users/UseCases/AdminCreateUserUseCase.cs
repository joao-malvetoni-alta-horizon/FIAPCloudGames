using FCG.Application.Users.DTOs;
using FCG.Application.Users.Interfaces;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FCG.Domain.Users.ValueObjects;

namespace FCG.Application.Users.UseCases;

public class AdminCreateUserUseCase(
    IUserUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IUserService userService) : IAdminCreateUserUseCase
{
    public async Task<AdminCreateUserResponse> ExecuteAsync(
        AdminCreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        await userService.CheckEmailUniquenessAsync(request.Email, cancellationToken);

        var role = await unitOfWork.Roles.GetByIdAsync(request.RoleId, cancellationToken)
                   ?? throw new UserDomainException($"Role '{request.RoleId}' not found.");

        Password.Validate(request.Password);
        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, passwordHash, role.Id);

        await unitOfWork.Users.AddAsync(user, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return new AdminCreateUserResponse(user.Id, user.Name.Value, user.Email.Address, user.RoleId);
    }
}