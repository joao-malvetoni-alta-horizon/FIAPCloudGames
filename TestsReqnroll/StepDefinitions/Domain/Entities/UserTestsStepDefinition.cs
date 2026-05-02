using FCG.Domain.Users.Constants;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FluentAssertions;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
[Scope(Feature = "Gerenciamento de Usuários")]
public class UserTestsStepDefinition
{
    private User? _user;
    private Action? _createAction;
    private readonly string _validHash = "$2a$12$somehashvalue";

    [When(@"eu criar um usuário com nome ""(.*)"", e-mail ""(.*)"" e cargo ""(.*)""")]
    public void WhenCriarUsuarioComum(string name, string email, RoleType role)
    {
        _user = User.Create(name, email, _validHash, role.ToRoleId());
    }

    [Then(@"o usuário deve estar ativo e com os dados preenchidos corretamente")]
    public void ThenValidarUsuarioAtivo()
    {
        _user.Should().NotBeNull();
        _user!.IsActive.Should().BeTrue();
        _user.Id.Should().NotBeEmpty();
    }

    [When(@"eu tentar criar um usuário com ""(.*)"" inválido: ""(.*)""")]
    public void WhenTentarCriarInvalido(string campo, string valor)
    {
        var validRoleId = RoleType.User.ToRoleId();

        _createAction = campo switch
        {
            "Nome" => () => User.Create(valor, "user@example.com", _validHash, validRoleId),
            "Email" => () => User.Create("John", valor, _validHash, validRoleId),
            "RoleId" => () => User.Create("John", "user@example.com", _validHash, Guid.Empty),
            _ => throw new ArgumentException("Campo não mapeado")
        };
    }

    [Then(@"deve ser lançada uma exceção de domínio de usuário contendo ""(.*)""")]
    public void ThenValidarMensagemExcecao(string mensagem)
    {
        _createAction.Should().Throw<UserDomainException>()
            .WithMessage($"*{mensagem}*");
    }

    [Given(@"que existe um usuário ""(.*)"" cadastrado")]
    public void GivenUsuarioCadastrado(string name)
    {
        _user = User.Create(name, "user@example.com", _validHash, RoleType.User.ToRoleId());
    }

    [When(@"eu desativar o usuário")]
    public void WhenDesativar()
    {
        _user!.Deactivate();
    }

    [Then(@"o usuário deve estar inativo")]
    public void ThenUsuarioInativo()
    {
        _user!.IsActive.Should().BeFalse();
    }

    [When(@"eu ativar o usuário")]
    public void WhenAtivar()
    {
        _user!.Activate();
    }

    [Then(@"o usuário deve estar ativo novamente")]
    public void ThenAtivoNovamente()
    {
        _user!.IsActive.Should().BeTrue();
        _user!.UpdatedAt.Should().NotBeNull();
    }

    [When(@"eu realizar o soft delete do usuário")]
    public void WhenSoftDelete()
    {
        _user!.SoftDelete();
    }

    [Then(@"a data de exclusão deve ser registrada")]
    public void ThenDataExclusao()
    {
        _user!.DeletedAt.Should().NotBeNull();
    }

    [Given(@"que existe um usuário ""(.*)"" cadastrado com cargo ""(.*)""")]
    public void GivenUsuarioComCargo(string name, RoleType role)
    {
        _user = User.Create(name, "user@example.com", _validHash, role.ToRoleId());
    }

    [When(@"eu alterar o cargo para ""(.*)""")]
    public void WhenAlterarCargo(RoleType newRole)
    {
        _user!.ChangeRole(newRole.ToRoleId());
    }

    [Then(@"o cargo do usuário deve ser atualizado com sucesso")]
    public void ThenCargoAtualizado()
    {
        _user!.UpdatedAt.Should().NotBeNull();
    }

    [When(@"eu criar o administrador raiz do sistema")]
    public void WhenCriarRootAdmin()
    {
        _user = User.CreateRootAdmin("Root", "root@example.com", _validHash, RoleType.Administrator.ToRoleId());
    }

    [Then(@"o ID do usuário deve ser o ID padrão de semente do sistema")]
    public void ThenValidarRootId()
    {
        _user!.Id.Should().Be(UserSeedConstants.RootAdminId);
    }
}