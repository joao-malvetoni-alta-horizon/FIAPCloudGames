using FCG.Application.Users.DTOs;
using FCG.Application.Users.UseCases;
using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using FCG.Domain.Users.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.TestsReqnroll.Application.Users.UseCases;

[Binding]
[Scope(Feature = "Listagem Paginada de Usuários")]
public class ListUsersStepDefinitions
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private PagedUsersResponse? _response;
    private List<User> _users = new();
    private int _totalCount;

    [Given(@"que existem usuários cadastrados no sistema:")]
    public void GivenUsuariosCadastrados(Table table)
    {
        foreach (var row in table.Rows)
        {
            _users.Add(User.Create(row["Nome"], row["Email"], "hash", RoleType.User.ToRoleId()));
        }
        _totalCount = _users.Count;
    }

    [Given(@"que o sistema possui usuários cadastrados")]
    public void GivenSistemaPossuiUsuarios()
    {
        _userRepoMock
            .Setup(r => r.ListAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<User>)[], 0));
    }

    [Given(@"que não existem usuários no sistema")]
    public void GivenNaoExistemUsuarios()
    {
        _users = new List<User>();
        _totalCount = 0;
    }

    [Given(@"que existe um usuário chamado ""(.*)"" sem cargo carregado")]
    public void GivenUsuarioSemCargo(string nome)
    {
        var user = User.Create(nome, "norole@example.com", "hash", RoleType.User.ToRoleId());
        _users.Add(user);
        _totalCount = 1;
    }

    [When(@"eu solicitar a listagem na página (.*) com tamanho (.*)")]
    public async Task WhenSolicitarListagem(int page, int size)
    {
        _userRepoMock
            .Setup(r => r.ListAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<User>)_users, _totalCount));

        var useCase = new ListUsersUseCase(_userRepoMock.Object);
        _response = await useCase.ExecuteAsync(page, size);
    }

    [Then(@"a resposta deve conter (.*) itens")]
    public void ThenRespostaContemItens(int qtd)
    {
        _response!.Items.Should().HaveCount(qtd);
    }

    [Then(@"o total de registros deve ser (.*)")]
    public void ThenTotalRegistros(int total)
    {
        _response!.TotalCount.Should().Be(total);
    }

    [Then(@"a página atual deve ser (.*)")]
    public void ThenPaginaAtual(int page)
    {
        _response!.Page.Should().Be(page);
    }

    [Then(@"o tamanho da página deve ser (.*)")]
    public void ThenTamanhoPagina(int size)
    {
        _response!.PageSize.Should().Be(size);
    }

    [Then(@"a lista deve conter o e-mail ""(.*)""")]
    public void ThenListaContemEmail(string email)
    {
        _response!.Items.Select(i => i.Email).Should().Contain(email);
    }

    [Then(@"a página resultante deve ser (.*)")]
    public void ThenPaginaResultante(int page)
    {
        _response!.Page.Should().Be(page);
    }

    [Then(@"o tamanho da página resultante deve ser (.*)")]
    public void ThenTamanhoResultante(int size)
    {
        _response!.PageSize.Should().Be(size);
    }

    [Then(@"o repositório deve ser consultado com (.*) e (.*)")]
    public void ThenRepositorioConsultado(int expectedPage, int expectedSize)
    {
        _userRepoMock.Verify(r => r.ListAsync(expectedPage, expectedSize, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Then(@"a lista de itens deve vir vazia")]
    public void ThenListaVazia()
    {
        _response!.Items.Should().BeEmpty();
    }

    [Then(@"o nome do cargo para este usuário deve ser vazio")]
    public void ThenNomeCargoVazio()
    {
        _response!.Items.Single().Role.Should().BeEmpty();
    }
}