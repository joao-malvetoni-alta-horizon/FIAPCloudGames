using FCG.Application.Games.DTOs;
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
[Scope(Feature = "Atualização de Promoção de Jogos")]
public class UpdatePromotionStepDefinitions
{
    private readonly Mock<IGamePromotionRepository> _promoRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private Guid _adminRoleId;
    private Guid _userRoleId;
    private GamePromotion? _promo;
    private PromotionResponse? _response;
    private Func<Task>? _action;

    private readonly DateTime _start = DateTime.UtcNow.AddDays(1);
    private readonly DateTime _end = DateTime.UtcNow.AddDays(10);

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

    [Given(@"que existe uma promoção cadastrada no sistema")]
    public void GivenQueExisteUmaPromocaoCadastrada()
    {
        _promo = GamePromotion.Create(Guid.NewGuid(), DiscountType.Percentage, 10, _start, _end);
        _promoRepoMock
            .Setup(r => r.GetByIdAsync(_promo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_promo);
    }

    [Given(@"que uma promoção com ID específico não existe no sistema")]
    public void GivenQueUmaPromocaoInexistente()
    {
        _promoRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GamePromotion?)null);
    }

    [Given(@"existe uma outra promoção ativa que sobrepõe o novo período")]
    public void GivenExisteSobreposicao()
    {
        _promoRepoMock
            .Setup(r => r.HasOverlappingActivePromotionAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    [Given(@"não existem promoções sobrepostas para o novo período")]
    public void GivenNaoExisteSobreposicao()
    {
        _promoRepoMock
            .Setup(r => r.HasOverlappingActivePromotionAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    [When(@"um usuário comum tentar atualizar essa promoção para (.*)% de desconto")]
    public void WhenUsuarioComumTentaAtualizar(int percent)
    {
        var request = new UpdatePromotionRequest(null, null, null, null, null);
        PrepareExecution(_promo!.Id, request, _userRoleId);
    }

    [When(@"um administrador tentar atualizar uma promoção inexistente para (.*)% de desconto")]
    public void WhenAdminTentaAtualizarInexistente(int percent)
    {
        var request = new UpdatePromotionRequest(null, null, null, null, null);
        PrepareExecution(Guid.NewGuid(), request, _adminRoleId);
    }

    [When(@"um administrador tentar atualizar essa promoção estendendo o período")]
    public void WhenAdminTentaEstenderPeriodo()
    {
        var newEnd = _end.AddDays(5);
        var request = new UpdatePromotionRequest(null, null, null, newEnd, null);
        PrepareExecution(_promo!.Id, request, _adminRoleId);
    }

    [When(@"um administrador tentar atualizar essa promoção para um valor fixo de (.*)")]
    public void WhenAdminTentaAtualizarValorFixo(decimal valor)
    {
        var request = new UpdatePromotionRequest(DiscountType.FixedValue, valor, null, null, null);
        PrepareExecution(_promo!.Id, request, _adminRoleId);
    }

    [When(@"um administrador solicitar a desativação da promoção")]
    public void WhenAdminDesativaPromocao()
    {
        var request = new UpdatePromotionRequest(null, null, null, null, false);
        PrepareExecution(_promo!.Id, request, _adminRoleId);
    }

    private void PrepareExecution(Guid promoId, UpdatePromotionRequest request, Guid roleId)
    {
        var useCase = new UpdatePromotionUseCase(_promoRepoMock.Object, _uowMock.Object);
        _action = async () => _response = await useCase.ExecuteAsync(promoId, request, roleId);
    }

    [Then(@"deve ser lançada uma exceção de permissão insuficiente")]
    public async Task ThenErroPermissao() => await _action!.Should().ThrowAsync<InsufficientGameManagementPermissionException>();

    [Then(@"deve ser lançada uma exceção de promoção não encontrada")]
    public async Task ThenErroNaoEncontrada() => await _action!.Should().ThrowAsync<PromotionNotFoundException>();

    [Then(@"deve ser lançada uma exceção de sobreposição de promoção")]
    public async Task ThenErroSobreposicao() => await _action!.Should().ThrowAsync<OverlappingPromotionException>();

    [Then(@"a promoção deve ser atualizada com sucesso")]
    public async Task ThenSucesso()
    {
        await _action!.Invoke();
        _response.Should().NotBeNull();
        _promoRepoMock.Verify(r => r.Update(It.IsAny<GamePromotion>()), Times.Once);
    }

    [Then(@"a promoção deve ser desativada com sucesso")]
    public async Task ThenDesativadaSucesso()
    {
        await _action!.Invoke();
        _response!.IsActive.Should().BeFalse();
    }

    [Then(@"os detalhes da promoção devem refletir o novo valor de (.*)")]
    public void ThenValidarValor(decimal valor)
    {
        _response!.DiscountValue.Should().Be(valor);
        _response.DiscountType.Should().Be(DiscountType.FixedValue);
    }

    [Then(@"o sistema não deve validar a sobreposição de promoções")]
    public void ThenNaoValidarSobreposicao()
    {
        _promoRepoMock.Verify(r => r.HasOverlappingActivePromotionAsync(
            It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Then(@"as alterações não devem ser persistidas no banco")]
    public void ThenNaoCommit() => _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

    [Then(@"as alterações devem ser persistidas no banco uma única vez")]
    public void ThenCommitUmaVez() => _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
}