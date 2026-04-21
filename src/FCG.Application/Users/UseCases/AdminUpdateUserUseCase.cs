using FCG.Application.Users.DTOs;
using FCG.Application.Users.Interfaces;
using FCG.Domain.Users.Constants;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;

namespace FCG.Application.Users.UseCases;

public class AdminUpdateUserUseCase(IUserUnitOfWork unitOfWork) : IAdminUpdateUserUseCase
{
    public async Task<AdminUpdateUserResponse> ExecuteAsync(
        Guid userId,
        AdminUpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
                   ?? throw new UserNotFoundException(userId);

        if (user.Id == UserSeedConstants.RootAdminId)
            throw new RootAdminOperationForbiddenException();

        if (request.RoleId.HasValue)
        {
            var role = await unitOfWork.Roles.GetByIdAsync(request.RoleId.Value, cancellationToken)
                       ?? throw new UserDomainException($"Role '{request.RoleId.Value}' não encontrado.");
            user.ChangeRole(role.Id);
        }

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value) user.Activate();
            else user.Deactivate();
        }

        unitOfWork.Users.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);

        return new AdminUpdateUserResponse(user.Id, user.Name.Value, user.Email.Address, user.IsActive, user.RoleId);
    }
}