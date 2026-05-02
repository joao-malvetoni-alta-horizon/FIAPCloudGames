using System.Text;
using System.Text.Json;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Infrastructure.Security;
using FluentAssertions;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
[Scope(Feature = "Geração de Token JWT")]
public class JwtTokenServiceTestsStepDefinitions
{
    private readonly JwtSettings _settings = new()
    {
        SecretKey = "test-secret-key-with-at-least-32-chars!!",
        ExpirationHours = 1
    };

    private User? _user;
    private string? _token;
    private string? _decodedPayload;
    private DateTimeOffset _before;

    [Given(@"que eu tenho um usuário com o cargo ""(.*)""")]
    public void GivenUsuarioComCargo(string roleName)
    {
        var role = Enum.Parse<RoleType>(roleName, true);
        _user = User.Create("Test User", "test@fcg.com", "$2a$12$somehashvalue", role.ToRoleId());
    }

    [Given(@"a configuração de expiração é de (.*) hora")]
    public void GivenConfiguracaoExpiracao(int hours)
    {
        _settings.ExpirationHours = hours;
    }

    [When(@"eu gerar o token de autenticação")]
    public void WhenGerarToken()
    {
        var service = new JwtTokenService(_settings);
        _before = DateTimeOffset.UtcNow;
        _token = service.GenerateToken(_user!);
        _decodedPayload = DecodePayload(_token);
    }

    [Then(@"o token deve retornar um formato JWT válido")]
    public void ThenValidarFormato()
    {
        _token.Should().NotBeNullOrEmpty();
        _token!.Split('.').Should().HaveCount(3, "JWT must have header.payload.signature format");
    }

    [Then(@"o payload deve conter o nome do cargo ""(.*)""")]
    public void ThenValidarRoleName(string roleName)
    {
        var role = Enum.Parse<RoleType>(roleName, true);
        _decodedPayload.Should().Contain(role.ToRoleName());
    }

    [Then(@"o payload deve conter o ID do usuário")]
    public void ThenValidarUserId()
    {
        _decodedPayload.Should().Contain(_user!.Id.ToString());
    }

    [Then(@"o payload deve conter o e-mail do usuário")]
    public void ThenValidarUserEmail()
    {
        _decodedPayload.Should().Contain(_user!.Email.Address);
    }

    [Then(@"o payload deve conter a claim ""(.*)"" com o valor ""(.*)""")]
    public void ThenValidarRoleIdClaim(string claimName, string valorEsperado)
    {
        _decodedPayload.Should().Contain(claimName);
        _decodedPayload.Should().Contain(valorEsperado);
    }

    [Then(@"o payload deve conter o ID do cargo de administrador correspondente a ""(.*)""")]
    public void ThenValidarAdminRoleId(string roleId)
    {
        _decodedPayload.Should().Contain(roleId);
    }

    [Then(@"a expiração deve ser definida com base nas configurações")]
    public void ThenValidarExpiracao()
    {
        var after = DateTimeOffset.UtcNow;
        var doc = JsonDocument.Parse(_decodedPayload!);
        var exp = doc.RootElement.GetProperty("exp").GetInt64();
        var expTime = DateTimeOffset.FromUnixTimeSeconds(exp);

        expTime.Should().BeAfter(_before.AddHours(1).AddSeconds(-5));
        expTime.Should().BeBefore(after.AddHours(1).AddSeconds(5));
    }

    private static string DecodePayload(string token)
    {
        var part = token.Split('.')[1];
        part = part.Replace('-', '+').Replace('_', '/');
        var mod4 = part.Length % 4;
        if (mod4 != 0) part += new string('=', 4 - mod4);
        return Encoding.UTF8.GetString(Convert.FromBase64String(part));
    }
}