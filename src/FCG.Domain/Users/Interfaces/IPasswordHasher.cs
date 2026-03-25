namespace FCG.Domain.Users.Interfaces;

/// <summary>Abstração para hashing e verificação de senhas em texto puro.</summary>
public interface IPasswordHasher
{
    /// <summary>Gera o hash de uma senha em texto puro.</summary>
    /// <param name="plainText">A senha bruta.</param>
    /// <returns>O hash BCrypt gerado.</returns>
    string Hash(string plainText);

    /// <summary>Verifica se uma senha em texto puro corresponde a um hash previamente gerado.</summary>
    /// <param name="plainText">A senha bruta a ser verificada.</param>
    /// <param name="hash">O hash BCrypt armazenado.</param>
    /// <returns><c>true</c> se a senha corresponder; caso contrário <c>false</c>.</returns>
    bool Verify(string plainText, string hash);
}
