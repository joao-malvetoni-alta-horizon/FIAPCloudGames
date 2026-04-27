using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FCG.Application.Configuration;
using FCG.Domain.Users.Entities;
using Microsoft.IdentityModel.Tokens;

namespace FCG.Application.Users.Interfaces;

/// <summary>
/// Interface para serviço de JWT
/// </summary>
public interface IJwtService
{
    string GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}

/// <summary>
/// Serviço para geração e validação de tokens JWT
/// </summary>
public class JwtService : IJwtService
{
    private readonly JwtOptions _jwtOptions;

    public JwtService(JwtOptions jwtOptions)
    {
        _jwtOptions = jwtOptions;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.UserName),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtOptions.ExpiryInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}