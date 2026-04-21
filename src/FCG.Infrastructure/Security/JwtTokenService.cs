using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FCG.Application.Auth.Interfaces;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using Microsoft.IdentityModel.Tokens;

namespace FCG.Infrastructure.Security;

public class JwtTokenService(JwtSettings settings) : IJwtTokenService
{
    public string GenerateToken(User user)
    {
        var roleType = user.RoleId == RoleType.Administrator.ToRoleId() ? RoleType.Administrator : RoleType.User;
        var roleName = roleType.ToRoleName();

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email.Address),
            new(ClaimTypes.Name, user.Name.Value),
            new(ClaimTypes.Role, roleName),
            new("roleId", user.RoleId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(settings.ExpirationHours),
            signingCredentials: credentials);


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}