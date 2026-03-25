using FCG.Domain.Users.Interfaces;

namespace FCG.Infrastructure.Security;

/// <summary>Implementação BCrypt de <see cref="IPasswordHasher"/> com fator de trabalho 12.</summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    /// <inheritdoc/>
    public string Hash(string plainText)
        => BCrypt.Net.BCrypt.HashPassword(plainText, WorkFactor);

    /// <inheritdoc/>
    public bool Verify(string plainText, string hash)
        => BCrypt.Net.BCrypt.Verify(plainText, hash);
}
