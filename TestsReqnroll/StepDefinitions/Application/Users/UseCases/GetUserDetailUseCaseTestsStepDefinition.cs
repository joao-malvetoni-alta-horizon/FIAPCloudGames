using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Exceptions;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;
using System.Reflection;

namespace FCG.TestsReqnroll.Application.Users.UseCases;

[Binding]
[Scope(Feature = "Obter Detalhes do Usuário")]
public class GetUserDetailUseCaseTestsStepDefinition
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private User? _user;
    private Guid _targetId;
    private UserDetailResponse? _response;
    private Func<Task>? _action;

    [Given(@"que um usuário com ID específico não existe no sistema")]
    public void GivenUsuarioInexistente()
    {
        _targetId = Guid.NewGuid();
        _userRepoMock
            .Setup(r => r.GetWithOwnedGamesAsync(_targetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
    }

    [Given(@"que existe um usuário chamado ""(.*)"" com e-mail ""(.*)""")]
    public void GivenUsuarioExiste(string nome, string email)
    {
        _user = User.Create(nome, email, "hash", RoleType.User.ToRoleId());
        _targetId = _user.Id;
        _userRepoMock
            .Setup(r => r.GetWithOwnedGamesAsync(_targetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user);
    }

    [Given(@"que existe um usuário cadastrado no sistema")]
    public void GivenUsuarioGenerico()
    {
        GivenUsuarioExiste("User", "user@example.com");
    }

    [Given(@"este usuário não possui jogos em sua biblioteca")]
    public void GivenSemJogos()
    {
    }

    [Given(@"a entidade de cargo \(Role\) não foi carregada para este usuário")]
    public void GivenSemRoleCarregada()
    {
    }

    [Given(@"que existe um usuário chamado ""(.*)"" com jogos em sua biblioteca")]
    public async Task GivenGamerComJogos(string nome)
    {
        _user = User.Create(nome, "gamer@example.com", "hash", RoleType.User.ToRoleId());
        _targetId = _user.Id;

        var game1 = UserOwnedGame.Create(_targetId, Guid.NewGuid(), 29.99m);
        await Task.Delay(10); 
        var game2 = UserOwnedGame.Create(_targetId, Guid.NewGuid(), 49.99m);

        var field = typeof(User).GetField("_ownedGames", BindingFlags.NonPublic | BindingFlags.Instance);
        var ownedGames = (List<UserOwnedGame>)field!.GetValue(_user)!;
        ownedGames.Add(game1);
        ownedGames.Add(game2);

        _userRepoMock
            .Setup(r => r.GetWithOwnedGamesAsync(_targetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user);
    }

    [Given(@"os jogos foram adquiridos em momentos diferentes")]
    public void GivenMomentosDiferentes()
    {
    }

    [When(@"eu solicitar os detalhes deste usuário")]
    public async Task WhenSolicitarDetalhes()
    {
        var useCase = new GetUserDetailUseCase(_userRepoMock.Object);
        _action = async () => _response = await useCase.ExecuteAsync(_targetId);

        if (_user != null) await _action();
    }

    [Then(@"deve ser lançada uma exceção de usuário não encontrado")]
    public async Task ThenErroNaoEncontrado()
    {
        await _action!.Should().ThrowAsync<UserNotFoundException>();
    }

    [Then(@"os dados retornados devem conter o nome ""(.*)"" e o e-mail ""(.*)""")]
    public void ThenValidarDadosBase(string nome, string email)
    {
        _response!.Name.Should().Be(nome);
        _response.Email.Should().Be(email);
    }

    [Then(@"a lista de jogos adquiridos deve estar vazia")]
    public void ThenListaVazia()
    {
        _response!.OwnedGames.Should().BeEmpty();
    }

    [Then(@"o campo de cargo na resposta deve estar vazio")]
    public void ThenRoleVazia()
    {
        _response!.Role.Should().BeEmpty();
    }

    [Then(@"a lista de jogos deve ser retornada em ordem decrescente pela data de aquisição")]
    public void ThenValidarOrdenacao()
    {
        _response!.OwnedGames.First().AcquiredAt
            .Should().BeOnOrAfter(_response.OwnedGames.Last().AcquiredAt);
    }

    [Then(@"a quantidade total de jogos deve ser (.*)")]
    public void ThenValidarQuantidade(int qtd)
    {
        _response!.OwnedGames.Should().HaveCount(qtd);
    }
}