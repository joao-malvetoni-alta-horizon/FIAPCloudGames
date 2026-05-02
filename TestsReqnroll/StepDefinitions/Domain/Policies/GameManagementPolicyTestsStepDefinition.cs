using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Policies;
using FCG.Domain.Users.Enums;
using FluentAssertions;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
public class GameManagementPolicyTestsStepDefinition
{
    private Action? _policyAction;

    [When(@"eu verificar a permissão de gerenciamento para o perfil ""(.*)""")]
    public void WhenVerificarPermissao(string perfil)
    {
        Guid roleId = perfil switch
        {
            "Administrator" => RoleType.Administrator.ToRoleId(),
            "User" => RoleType.User.ToRoleId(),
            "Desconhecido" => Guid.NewGuid(),
            _ => throw new ArgumentException("Perfil não mapeado no teste")
        };

        _policyAction = () => GameManagementPolicy.EnsureCanManage(roleId);
    }

    [Then(@"o sistema deve permitir a ação: ""(.*)""")]
    public void ThenValidarPermissao(bool permitido)
    {
        if (permitido)
        {
            _policyAction.Should().NotThrow();
        }
        else
        {
            _policyAction.Should().Throw<InsufficientGameManagementPermissionException>();
        }
    }
}