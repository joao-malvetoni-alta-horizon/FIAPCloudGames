using FCG.Domain.Games.Entities;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Exceptions;
using FluentAssertions;

namespace FCG.TestsReqnroll.StepDefinitions;

[Binding]
[Scope(Feature = "Gerenciamento de Cadastro de Jogos")]
public class GameTestsStepDefinition
{
    private Game? _game;
    private Action? _createAction;
    private readonly DateOnly _futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

    [When(@"eu criar um novo jogo com título ""(.*)"", preço (.*) e gênero ""(.*)""")]
    public void WhenCriarNovoJogo(string title, decimal price, GameGenre genre)
    {
        _game = new Game(title, "A legendary sequel.", price, genre, _futureDate);
    }

    [Then(@"o jogo deve ser criado com status ""(.*)"" e os dados devem estar corretos")]
    public void ThenValidarDadosJogo(GameStatus status)
    {
        _game.Should().NotBeNull();
        _game!.Status.Should().Be(status);
        _game.Id.Should().NotBeEmpty();
    }

    [When(@"eu tentar cadastrar um jogo com ""(.*)"" inválido: ""(.*)""")]
    public void WhenTentarCadastrarInvalido(string campo, string valor)
    {
        _createAction = campo switch
        {
            "Titulo" => () => new Game(valor, "desc", 10m, GameGenre.RPG, _futureDate),
            "Preco" => () => new Game("Valid", "desc", decimal.Parse(valor), GameGenre.RPG, _futureDate),
            "Lancamento" => () => new Game("Valid", "desc", 10m, GameGenre.RPG, DateOnly.Parse(valor)),
            _ => throw new ArgumentException("Campo não mapeado")
        };
    }

    [Then(@"deve ser lançada a exceção ""(.*)""")]
    public void ThenValidarExcecao(string excecao)
    {
        _createAction.Should().Throw<Exception>()
            .And.GetType().Name.Should().Be(excecao);
    }

    [Given(@"que existe um jogo cadastrado com preço (.*)")]
    public void GivenJogoCadastrado(decimal price)
    {
        _game = new Game("Valid Title", "desc", price, GameGenre.RPG, _futureDate);
    }

    [When(@"eu atualizar o preço do jogo para (.*)")]
    public void WhenAtualizarPreco(decimal newPrice)
    {
        _game!.Update(price: newPrice);
    }

    [Then(@"o preço atual deve ser (.*)")]
    public void ThenPrecoAtual(decimal expectedPrice)
    {
        _game!.Price.Amount.Should().Be(expectedPrice);
    }

    [Then(@"a data de atualização deve ser preenchida")]
    public void ThenDataAtualizacao()
    {
        _game!.UpdatedAt.Should().NotBeNull();
    }
}