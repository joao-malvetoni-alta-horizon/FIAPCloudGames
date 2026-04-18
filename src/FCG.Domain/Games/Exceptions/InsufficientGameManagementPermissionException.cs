namespace FCG.Domain.Games.Exceptions;

public sealed class InsufficientGameManagementPermissionException : DomainException
{
    public InsufficientGameManagementPermissionException()
        : base("Only administrators can manage games.")
    {
    }
}
