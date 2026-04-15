using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Policies;
using FCG.Domain.Users.Enums;
using FluentAssertions;

namespace FCG.Tests.Domain.Policies;

public class GameManagementPolicyTests
{
    [Fact]
    public void EnsureCanManage_WhenRoleIsAdministrator_ShouldNotThrow()
    {
        var adminRoleId = RoleType.Administrator.ToRoleId();

        var act = () => GameManagementPolicy.EnsureCanManage(adminRoleId);

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureCanManage_WhenRoleIsUser_ShouldThrowInsufficientPermissionException()
    {
        var userRoleId = RoleType.User.ToRoleId();

        var act = () => GameManagementPolicy.EnsureCanManage(userRoleId);

        act.Should().Throw<InsufficientGameManagementPermissionException>();
    }

    [Fact]
    public void EnsureCanManage_WhenRoleIsUnknown_ShouldThrowInsufficientPermissionException()
    {
        var unknownRoleId = Guid.NewGuid();

        var act = () => GameManagementPolicy.EnsureCanManage(unknownRoleId);

        act.Should().Throw<InsufficientGameManagementPermissionException>();
    }
}
