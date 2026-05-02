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
[Scope(Feature = "Exclusão de Usuário por Administrador")]
public class AdminDeleteUserUseCaseTestsStepDefinition
{
    private readonly Mock<IUserUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();

    private User? _user;
    private Guid _targetId;
    private Func<Task>? _action;

    [Given(@"que o repositório de usuários está configurado no Unit of Work")]
    public void GivenRepositorioConfigurado()
    {
        _unitOfWorkMock.SetupGet(uow => uow.Users).Returns(_userRepoMock.Object);
    }

    [Given(@"que um usuário com ID específico não existe no sistema")]
    public void GivenUsuarioNaoExiste()
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
        _user = User.CreateRootAdmin("Root", "root@example.com", "hash", RoleType.Administrator.ToRoleId());

        _userRepoMock
            .Setup(r => r.GetByIdAsync(_targetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user);
    }

    [Given(@"que existe um usuário comum cadastrado no sistema")]
    public void GivenUsuarioComumExiste()
    {
        _user = User.Create("Some User", "some@example.com", "hash", RoleType.User.ToRoleId());
        _targetId = _user.Id;

        _userRepoMock
            .Setup(r => r.GetByIdAsync(_targetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user);
    }

    [When(@"o administrador solicitar a exclusão deste usuário")]
    public void WhenSolicitarExclusao()
    {
        var useCase = new AdminDeleteUserUseCase(_unitOfWorkMock.Object);
        _action = async () => await useCase.ExecuteAsync(_targetId);
    }

    [Then(@"deve ser lançada uma exceção de usuário não encontrado")]
    public async Task ThenErroUsuarioNaoEncontrado()
    {
        await _action!.Should().ThrowAsync<UserNotFoundException>();
    }

    [Then(@"deve ser lançada uma exceção de operação proibida para o Root Admin")]
    public async Task ThenErroOperacaoProibida()
    {
        await _action!.Should().ThrowAsync<RootAdminOperationForbiddenException>();
    }

    [Then(@"o usuário deve ser marcado como inativo")]
    public async Task ThenUsuarioInativo()
    {
        await _action!.Invoke();
        _user!.IsActive.Should().BeFalse();
    }

    [Then(@"a data de exclusão deve ser registrada")]
    public void ThenDataExclusaoRegistrada()
    {
        _user!.DeletedAt.Should().NotBeNull();
    }

    [Then(@"o status do usuário não deve ser alterado no banco")]
    public void ThenStatusNaoAlterado()
    {
        _userRepoMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Then(@"as alterações de usuário não devem ser persistidas")]
    public void ThenNaoPersistir()
    {
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Then(@"as alterações de usuário devem ser persistidas uma única vez")]
    public void ThenPersistirUmaVez()
    {
        _userRepoMock.Verify(r => r.Update(_user!), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}