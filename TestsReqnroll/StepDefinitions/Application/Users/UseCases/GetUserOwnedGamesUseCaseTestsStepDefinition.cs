using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Games.Entities;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.TestsReqnroll.Application.Users.UseCases;

[Binding]
public class GetUserOwnedGamesUseCaseTestsStepDefinition
{
    private readonly ScenarioContext _scenarioContext;
    private readonly Mock<IUserUnitOfWork> _uowMock = new();
    private readonly Mock<IUserRepository> _usersMock = new();
    private readonly Mock<IUserOwnedGameRepository> _librariesMock = new();

    private readonly Guid _userId = Guid.NewGuid();
    private List<UserOwnedGameResponse>? _response;

    public GetUserOwnedGamesUseCaseTestsStepDefinition(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;

        _uowMock.SetupGet(u => u.Users).Returns(_usersMock.Object);
        _uowMock.SetupGet(u => u.UserOwnedGames).Returns(_librariesMock.Object);
    }

    [Given(@"que o Unit of Work fornece acesso aos repositórios de usuários e biblioteca")]
    public void GivenUowConfigurado() { }

    [Given(@"que o ID do usuário informado não corresponde a nenhum registro")]
    public void GivenUsuarioInexistente()
    {
        _usersMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
    }

    [Given(@"que existe um usuário ativo cadastrado no sistema")]
    public void GivenUsuarioAtivo()
    {
        var user = User.Create("John Doe", "john@example.com", "$2a$12$hash", RoleType.User.ToRoleId());

        _usersMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
    }

    [Given(@"que este usuário adquiriu os seguintes jogos:")]
    public void GivenAdquiriuJogos(Table table)
    {
        var ownedGames = new List<UserOwnedGame>();

        foreach (var row in table.Rows)
        {
            var preco = decimal.Parse(row["Preco"]);
            ownedGames.Add(UserOwnedGame.Create(_userId, Guid.NewGuid(), preco));
            Thread.Sleep(20);
        }

        _librariesMock.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ownedGames);
    }

    [Given(@"o usuário ainda não adquiriu nenhum jogo")]
    public void GivenNaoPossuiJogos()
    {
        _librariesMock.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserOwnedGame>());
    }

    [When(@"eu solicitar a lista de jogos deste usuário")]
    public async Task WhenSolicitarLista()
    {
        var useCase = new GetUserOwnedGamesUseCase(_uowMock.Object);

        try
        {
            var result = await useCase.ExecuteAsync(_userId);
            _response = result?.ToList() ?? new List<UserOwnedGameResponse>();
        }
        catch (Exception ex)
        {
            _scenarioContext["Exception"] = ex;
        }
    }

    [Then(@"deve ser lançada uma exceção de usuário não encontrado")]
    public void ThenErroUsuarioNaoEncontrado()
    {
        _scenarioContext.Should().ContainKey("Exception");
        _scenarioContext["Exception"].Should().BeOfType<UserNotFoundException>();
    }

    [Then(@"a resposta deve conter (.*) jogos")]
    public void ThenRespostaContemQuantidade(int count)
    {
        if (_scenarioContext.ContainsKey("Exception"))
            throw (Exception)_scenarioContext["Exception"];

        _response.Should().NotBeNull();
        _response.Should().HaveCount(count);
    }

    [Then(@"o primeiro jogo da lista deve ser o mais recente com preço (.*)")]
    public void ThenPrimeiroJogo(decimal preco)
    {
        _response.Should().NotBeEmpty();
        _response![0].PricePaid.Should().Be(preco);
    }

    [Then(@"o segundo jogo da lista deve ser o mais antigo com preço (.*)")]
    public void ThenSegundoJogo(decimal preco)
    {
        _response.Should().HaveCount(2);
        _response![1].PricePaid.Should().Be(preco);
    }

    [Then(@"a resposta deve ser uma lista vazia")]
    public void ThenRespostaVazia()
    {
        _response.Should().BeEmpty();
    }
}