namespace FCG.Domain.Users.Enums;

/// <summary>Métodos de extensão que mapeiam <see cref="RoleType"/> para Guids de seed e nomes de exibição.</summary>
public static class RoleTypeExtensions
{
    // Guids determinísticos usados nos dados de seed — nunca altere estes valores.
    private static readonly Guid UserRoleId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid AdministratorRoleId = new("22222222-2222-2222-2222-222222222222");

    /// <summary>Retorna o Guid fixo de seed para este perfil.</summary>
    public static Guid ToRoleId(this RoleType role) => role switch
    {
        RoleType.User => UserRoleId,
        RoleType.Administrator => AdministratorRoleId,
        _ => throw new ArgumentOutOfRangeException(nameof(role))
    };

    /// <summary>Retorna o nome de exibição em português para este perfil.</summary>
    public static string ToRoleName(this RoleType role) => role switch
    {
        RoleType.User => "Usuário",
        RoleType.Administrator => "Administrador",
        _ => throw new ArgumentOutOfRangeException(nameof(role))
    };
}
