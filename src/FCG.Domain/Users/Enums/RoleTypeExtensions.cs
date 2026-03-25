namespace FCG.Domain.Users.Enums;

public static class RoleTypeExtensions
{
    // Guids determinísticos usados nos dados de seed — nunca altere estes valores.
    private static readonly Guid UserRoleId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid AdministratorRoleId = new("22222222-2222-2222-2222-222222222222");
    public static Guid ToRoleId(this RoleType role) => role switch
    {
        RoleType.User => UserRoleId,
        RoleType.Administrator => AdministratorRoleId,
        _ => throw new ArgumentOutOfRangeException(nameof(role))
    };
    public static string ToRoleName(this RoleType role) => role switch
    {
        RoleType.User => "Usuário",
        RoleType.Administrator => "Administrador",
        _ => throw new ArgumentOutOfRangeException(nameof(role))
    };
}
