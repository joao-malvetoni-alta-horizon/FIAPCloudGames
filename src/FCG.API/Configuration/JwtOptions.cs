namespace FCG.API.Configuration;

/// <summary>
/// Configurações JWT da aplicação
/// </summary>
public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiryInMinutes { get; set; }
}