using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FCG.Domain.Games.Interfaces;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.TestsReqnroll.Application.Users.UseCases;

[Binding]
[Scope(Feature = "Compra de Jogo pelo Usuário")]
public class PurchaseOwnedGameUseCaseTestsStepDefinition
{
    private readonly Mock<IUserUnitOfWork> _uowMock = new();
    private readonly Mock<IUserRepository> _usersMock = new();
    private readonly Mock<IUserOwnedGameRepository> _librariesMock = new();
    private readonly Mock<IGameRepository> _gamesMock = new();

    private Guid _userId = Guid.NewGuid();
    private Guid _gameId = Guid.NewGuid();
    private User? _user;
    private Game? _game;
    private PurchaseOwnedGameResponse? _response;
    private Func<Task>? _action;

    [Given(@"que o sistema de persistência \(UoW\) está operacional")]
    public void GivenUowConfigurado()
    {
        _uowMock.SetupGet(u => u.Users).Returns(_usersMock.Object);
        _uowMock.SetupGet(u => u.UserOwnedGames).Returns(_librariesMock.Object);
    }

    [Given(@"os repositórios de usuários, jogos e biblioteca estão configurados")]
    public void GivenRepositoriosConfigurados() { }

    [Given(@"que existe um usuário ativo no sistema")]
    public void GivenUsuarioAtivo()
    {
        _user = User.Create("John Doe", "john@example.com", "$2a$12$hash", RoleType.User.ToRoleId());
        _usersMock.Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>())).ReturnsAsync(_user);
    }

    [Given(@"que existe um usuário inativo no sistema")]
    public void GivenUsuarioInativo()
    {
        GivenUsuarioAtivo();
        _user!.Deactivate();
    }

    [Given(@"que o ID do usuário informado não corresponde a nenhum registro")]
    public void GivenUsuarioInexistente()
    {
        _usersMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
    }

    [Given(@"um jogo disponível com status ""(.*)"" e preço (.*)")]
    public void GivenJogoDisponivelComPreco(string statusName, decimal preco)
    {
        ConfigurarMockJogo(statusName, preco);
    }

    [Given(@"um jogo disponível com status ""(.*)""")]
    public void GivenJogoDisponivelSemPreco(string statusName)
    {
        ConfigurarMockJogo(statusName, 100m); 
    }

    private void ConfigurarMockJogo(string statusName, decimal preco)
    {
        Enum.TryParse<GameStatus>(statusName, out var status);
        _game = new Game("Cyber Runner", "Great game", preco, GameGenre.Action, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));

        if (status != GameStatus.Active) _game.Update(status: status);

        _gamesMock.Setup(r => r.GetByIdAsync(_gameId, It.IsAny<CancellationToken>())).ReturnsAsync(_game);
    }

    [Given(@"o usuário ainda não possui este jogo na biblioteca")]
    public void GivenNaoPossuiJogo()
    {
        _librariesMock.Setup(r => r.ExistsAsync(_userId, _gameId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
    }

    [Given(@"o usuário já possui este jogo em sua biblioteca")]
    public void GivenJaPossuiJogo()
    {
        _librariesMock.Setup(r => r.ExistsAsync(_userId, _gameId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
    }

    [When(@"o usuário solicitar a compra do jogo")]
    public void WhenSolicitaCompra()
    {
        var useCase = new PurchaseOwnedGameUseCase(_uowMock.Object, _gamesMock.Object);
        var request = new PurchaseOwnedGameRequest(_gameId);
        _action = async () => _response = await useCase.ExecuteAsync(_userId, request);
    }

    [Then(@"a compra deve ser registrada com o preço de (.*)")]
    public async Task ThenCompraRegistrada(decimal preco)
    {
        await _action!.Invoke();
        _response!.PricePaid.Should().Be(preco);
        _librariesMock.Verify(r => r.AddAsync(It.Is<UserOwnedGame>(g => g.PricePaid == preco), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Then(@"a transação deve ser persistida no banco de dados")]
    public void ThenPersisteNoBanco()
    {
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Then(@"deve ser lançada uma exceção de usuário não encontrado")]
    public async Task ThenErroUsuarioNaoEncontrado() => await _action!.Should().ThrowAsync<UserNotFoundException>();

    [Then(@"deve ser lançada uma exceção de domínio com a mensagem ""(.*)""")]
    public async Task ThenErroDominioMensagem(string msg)
    {
        await _action!.Should().ThrowAsync<UserDomainException>().WithMessage(msg);
    }

    [Then(@"deve ser lançada uma exceção de validação com a mensagem ""(.*)""")]
    public async Task ThenErroValidacaoMensagem(string msg)
    {
        await _action!.Should().ThrowAsync<DomainValidationException>().WithMessage(msg);
    }

    [Then(@"deve ser lançada uma exceção informando que o usuário já possui o jogo")]
    public async Task ThenErroJaPossui() => await _action!.Should().ThrowAsync<UserAlreadyOwnsGameException>();
}