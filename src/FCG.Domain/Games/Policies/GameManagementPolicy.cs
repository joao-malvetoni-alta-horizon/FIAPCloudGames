using FCG.Domain.Games.Exceptions;
using FCG.Domain.Users.Enums;

namespace FCG.Domain.Games.Policies;

public static class GameManagementPolicy
{
    public static void EnsureCanManage(Guid roleId)
    {
        if (roleId != RoleType.Administrator.ToRoleId())
            throw new InsufficientGameManagementPermissionException();
    }
}
