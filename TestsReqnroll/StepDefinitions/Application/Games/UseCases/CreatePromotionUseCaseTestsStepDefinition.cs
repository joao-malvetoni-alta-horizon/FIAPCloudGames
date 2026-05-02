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
[Scope(Feature = "Criação de Promoção de Jogos")]
public class CreatePromotionStepDefinitions
{
    private readonly Mock<IGameRepository> _gameRepoMock = new();
    private readonly Mock<IGamePromotionRepository> _promoRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private Guid _adminRoleId;
    private Guid _userRoleId;
    private Game _game = null!;
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

    [Given(@"que existe um jogo cadastrado para promoção")]
    public void GivenQueExisteUmJogoCadastrado()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        _game = new Game("Test Game", "Description", 29.99m, GameGenre.Action, future);

        _gameRepoMock
            .Setup(r => r.GetByIdAsync(_game.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_game);
    }

    [Given(@"que um jogo com ID específico não existe no sistema")]
    public void GivenQueUmJogoComIDEspecificoNaoExiste()
    {
        _gameRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Game?)null);
    }

    [Given(@"já existe uma promoção ativa para este jogo no mesmo período")]
    public void GivenJaExisteUmaPromocaoAtiva()
    {
        _promoRepoMock
            .Setup(r => r.HasOverlappingActivePromotionAsync(It.IsAny<Guid>(), _start, _end, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    [Given(@"não existem promoções sobrepostas para este jogo no período")]
    public void GivenNaoExistemPromocoesSobrepostas()
    {
        _promoRepoMock
            .Setup(r => r.HasOverlappingActivePromotionAsync(It.IsAny<Guid>(), _start, _end, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    [When(@"um usuário comum tentar criar uma promoção de (\d+)% de desconto")]
    public void WhenUmUsuarioComumTentarCriarUmaPromocao(int percent)
    {
        PrepareExecution(percent, _userRoleId);
    }

    [When(@"um administrador tentar criar uma promoção de (\d+)% de desconto")]
    public void WhenUmAdministradorTentarCriarUmaPromocao(int percent)
    {
        PrepareExecution(percent, _adminRoleId);
    }

    [When(@"um administrador tentar criar uma promoção para este ID inexistente")]
    public void WhenAdministradorTentaCriarParaInexistente()
    {
        PrepareExecution(20, _adminRoleId, Guid.NewGuid());
    }

    private void PrepareExecution(int percent, Guid roleId, Guid? customGameId = null)
    {
        var useCase = new CreatePromotionUseCase(_gameRepoMock.Object, _promoRepoMock.Object, _uowMock.Object);
        var request = new CreatePromotionRequest(DiscountType.Percentage, percent, _start, _end);
        var gameId = customGameId ?? (_game?.Id ?? Guid.Empty);

        _action = async () => _response = await useCase.ExecuteAsync(gameId, request, roleId);
    }

    [Then(@"deve ser lançada uma exceção de permissão insuficiente")]
    public async Task ThenDeveLancarPermissaoInsuficiente()
    {
        await _action!.Should().ThrowAsync<InsufficientGameManagementPermissionException>();
    }

    [Then(@"deve ser lançada uma exceção de jogo não encontrado")]
    public async Task ThenDeveLancarJogoNaoEncontrado()
    {
        await _action!.Should().ThrowAsync<GameNotFoundException>();
    }

    [Then(@"deve ser lançada uma exceção de sobreposição de promoção")]
    public async Task ThenDeveLancarSobreposicao()
    {
        await _action!.Should().ThrowAsync<OverlappingPromotionException>();
    }

    [Then(@"a promoção deve ser criada com sucesso")]
    public async Task ThenPromocaoCriadaComSucesso()
    {
        await _action!.Invoke();
        _response.Should().NotBeNull();
    }

    [Then(@"os detalhes da promoção devem refletir o desconto de (\d+)%")]
    public void ThenDetalhesRefletemDesconto(int percent)
    {
        _response!.DiscountValue.Should().Be(percent);
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