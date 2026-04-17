using FCG.Application.Users.DTOs;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Interfaces;
using FCG.Domain.Users.ValueObjects;

namespace FCG.Application.Users.UseCases;

public class RegisterUserUseCase(IUserUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IUserService userService)
{
    public async Task<RegisterUserResponse> ExecuteAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        await userService.CheckEmailUniquenessAsync(request.Email, cancellationToken);
        var roleId = RoleType.User.ToRoleId();

        Password.Validate(request.Password);
        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, passwordHash, roleId);

        await unitOfWork.Users.AddAsync(user, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return new RegisterUserResponse(user.Id, user.Name.Value, user.Email.Address, user.RoleId);
    }
}
