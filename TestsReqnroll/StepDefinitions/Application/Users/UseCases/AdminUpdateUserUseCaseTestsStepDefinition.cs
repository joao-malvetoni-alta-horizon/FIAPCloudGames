using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Constants;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.TestsReqnroll.Application.Users.UseCases;

[Binding]
[Scope(Feature = "Atualização de Usuário por Administrador")]
public class AdminUpdateUserUseCaseTestsStepDefinition
{
    private readonly Mock<IUserUnitOfWork> _uowMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IRoleRepository> _roleRepoMock = new();
    
    private static readonly Guid AdminRoleId = RoleType.Administrator.ToRoleId();
    private static readonly Guid UserRoleId = RoleType.User.ToRoleId();
    
    private User? _user;
    private Guid _targetId;
    private AdminUpdateUserResponse? _response;
    private Func<Task>? _action;

    [Given(@"que o Unit of Work está configurado com repositórios de usuários e cargos")]
    public void GivenUowConfigurado()
    {
        _uowMock.SetupGet(u => u.Users).Returns(_userRepoMock.Object);
        _uowMock.SetupGet(u => u.Roles).Returns(_roleRepoMock.Object);
    }

    [Given(@"que um usuário com ID específico não existe no sistema")]
    public void GivenUsuarioInexistente()
    {
        _targetId = Guid.NewGuid();
        _userRepoMock
            .Setup(r => r.GetByIdAsync(_targetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
    }

    [Given(@"que o usuário alvo é o Administrador Raiz do sistema")]
    public void GivenUsuarioAlvoERoot()
    {
        _targetId = UserSeedConstants.RootAdminId;
        _user = User.CreateRootAdmin("Root", "root@example.com", "hash", AdminRoleId);
        _userRepoMock.Setup(r => r.GetByIdAsync(_targetId, It.IsAny<CancellationToken>())).ReturnsAsync(_user);
    }

    [Given(@"que existe um usuário cadastrado no sistema")]
    [Given(@"que existe um usuário ativo no sistema")]
    public void GivenUsuarioExiste()
    {
        _user = User.Create("Test User", "test@example.com", "hash", UserRoleId);
        _targetId = _user.Id;
        _userRepoMock.Setup(r => r.GetByIdAsync(_targetId, It.IsAny<CancellationToken>())).ReturnsAsync(_user);
    }

    [Given(@"o cargo com ID informado não existe")]
    public void GivenCargoNaoExiste()
    {
        _roleRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Role?)null);
    }

    [Given(@"o cargo de ""(.*)"" existe no sistema")]
    public void GivenCargoExiste(string nomeCargo)
    {
        var role = Role.Create(nomeCargo);
        _roleRepoMock.Setup(r => r.GetByIdAsync(AdminRoleId, It.IsAny<CancellationToken>())).ReturnsAsync(role);
    }

    [When(@"o administrador tentar atualizar os dados deste usuário")]
    public void WhenTentaAtualizar()
    {
        PrepareExecution(new AdminUpdateUserRequest(null, null));
    }

    [When(@"o administrador tentar desativar o Administrador Raiz")]
    public void WhenDesativaRoot()
    {
        PrepareExecution(new AdminUpdateUserRequest(false, null));
    }

    [When(@"o administrador tentar mudar o cargo do usuário")]
    public void WhenMudaCargoInexistente()
    {
        PrepareExecution(new AdminUpdateUserRequest(null, Guid.NewGuid()));
    }

    [When(@"o administrador solicitar a desativação do usuário")]
    public void WhenSolicitaDesativacao()
    {
        PrepareExecution(new AdminUpdateUserRequest(false, null));
    }

    [When(@"o administrador solicitar a mudança de cargo do usuário")]
    public void WhenSolicitaMudancaCargo()
    {
        PrepareExecution(new AdminUpdateUserRequest(null, AdminRoleId));
    }

    [When(@"o administrador enviar uma atualização com campos nulos")]
    public void WhenUpdateNulo()
    {
        PrepareExecution(new AdminUpdateUserRequest(null, null));
    }

    private void PrepareExecution(AdminUpdateUserRequest request)
    {
        var useCase = new AdminUpdateUserUseCase(_uowMock.Object);
        _action = async () => _response = await useCase.ExecuteAsync(_targetId, request);
    }

    [Then(@"deve ser lançada uma exceção de usuário não encontrado")]
    public async Task ThenErroUsuarioNaoEncontrado() => await _action!.Should().ThrowAsync<UserNotFoundException>();

    [Then(@"deve ser lançada uma exceção de operação proibida para o Root Admin")]
    public async Task ThenErroOperacaoProibida() => await _action!.Should().ThrowAsync<RootAdminOperationForbiddenException>();

    [Then(@"deve ser lançada uma exceção de domínio de usuário")]
    public async Task ThenErroDominio() => await _action!.Should().ThrowAsync<UserDomainException>();

    [Then(@"o usuário deve ser marcado como inativo")]
    public async Task ThenInativo()
    {
        await _action!.Invoke();
        _response!.IsActive.Should().BeFalse();
    }

    [Then(@"o usuário deve passar a ter o novo cargo")]
    public async Task ThenNovoCargo()
    {
        await _action!.Invoke();
        _response!.RoleId.Should().NotBe(UserRoleId);
    }

    [Then(@"o estado do usuário deve permanecer o mesmo")]
    public async Task ThenEstadoMantido()
    {
        await _action!.Invoke();
        _response!.IsActive.Should().BeTrue();
        _response.RoleId.Should().Be(UserRoleId);
    }

    [Then(@"o sistema não deve buscar informações de cargo no banco")]
    public void ThenNaoBuscaCargo()
    {
        _roleRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Then(@"as alterações não devem ser persistidas no banco")]
    public void ThenNaoPersiste() => _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

    [Then(@"as alterações devem ser persistidas no banco uma única vez")]
    public void ThenPersisteUmaVez() => _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
}