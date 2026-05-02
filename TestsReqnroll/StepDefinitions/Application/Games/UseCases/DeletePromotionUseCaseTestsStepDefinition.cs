using FCG.Application.Games.UseCases;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Shared;
using FCG.Domain.Users.Enums;
using FluentAssertions;
using Moq;

namespace FCG.TestsReqnroll.Application.Games.UseCases;

[Binding]
[Scope(Feature = "Exclusão de Promoção de Jogos")]
public class DeletePromotionUseCaseTestsStepDefinition
{
    private readonly Mock<IGamePromotionRepository> _promoRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private Guid _adminRoleId;
    private Guid _userRoleId;
    private GamePromotion? _promo;
    private Func<Task>? _action;

    [Given(@"que o ID do cargo de administrador é definido")]
    public void GivenQueOIDDoCargoDeAdministradorEDefinido()
    {
        _adminRoleId = RoleType.Administrator.ToRoleId();
    }

    [Given(@"que o ID do cargo de usuário comum é definido")]
    public void GivenQueOIDDoCargoDeUsuarioComumEDefinido()
    {
        _userRoleId = RoleType.User.ToRoleId();
    }

    [Given(@"que uma promoção com ID específico não existe no sistema")]
    public void GivenQueUmaPromocaoComIDEspecificoNaoExiste()
    {
        _promoRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GamePromotion?)null);
    }

    [Given(@"que existe uma promoção cadastrada no sistema")]
    public void GivenQueExisteUmaPromocaoCadastrada()
    {
        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(10);
        _promo = GamePromotion.Create(Guid.NewGuid(), DiscountType.Percentage, 15, start, end);

        _promoRepoMock
            .Setup(r => r.GetByIdAsync(_promo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_promo);
    }

    [When(@"um usuário comum tentar excluir uma promoção existente")]
    public void WhenUmUsuarioComumTentarExcluir()
    {
        PrepareExecution(Guid.NewGuid(), _userRoleId);
    }

    [When(@"um administrador tentar excluir esta promoção inexistente")]
    public void WhenAdministradorTentarExcluirInexistente()
    {
        PrepareExecution(Guid.NewGuid(), _adminRoleId);
    }

    [When(@"um administrador tentar excluir esta promoção existente")]
    public void WhenAdministradorTentarExcluirExistente()
    {
        PrepareExecution(_promo!.Id, _adminRoleId);
    }

    private void PrepareExecution(Guid promoId, Guid roleId)
    {
        var useCase = new DeletePromotionUseCase(_promoRepoMock.Object, _uowMock.Object);
        _action = async () => await useCase.ExecuteAsync(promoId, roleId);
    }

    [Then(@"deve ser lançada uma exceção de permissão insuficiente")]
    public async Task ThenDeveLancarPermissaoInsuficiente()
    {
        await _action!.Should().ThrowAsync<InsufficientGameManagementPermissionException>();
    }

    [Then(@"deve ser lançada uma exceção de promoção não encontrada")]
    public async Task ThenDeveLancarPromocaoNaoEncontrada()
    {
        await _action!.Should().ThrowAsync<PromotionNotFoundException>();
    }

    [Then(@"a promoção deve ser removida com sucesso")]
    public async Task ThenPromocaoRemovidaComSucesso()
    {
        await _action!.Invoke();
        _promoRepoMock.Verify(r => r.Delete(_promo!), Times.Once);
    }

    [Then(@"as alterações não devem ser persistidas no banco")]
    public void ThenNaoDevePersistir()
    {
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Then(@"as alterações devem ser persistidas no banco uma única vez")]
    public void ThenDevePersistirUmaVez()
    {
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}