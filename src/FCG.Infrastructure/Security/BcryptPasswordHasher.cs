using FCG.Domain.Users.Interfaces;

namespace FCG.Infrastructure.Security;

public class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;
    public string Hash(string plainText)
        => BCrypt.Net.BCrypt.HashPassword(plainText, WorkFactor);

    public bool Verify(string plainText, string hash)
        => BCrypt.Net.BCrypt.Verify(plainText, hash);
}
