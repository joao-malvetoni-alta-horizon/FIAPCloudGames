# language: pt-br
Funcionalidade: Gerenciamento de Cadastro de Jogos

  Cenário: Cadastro de jogo com sucesso
    Quando eu criar um novo jogo com título "Half-Life 3", preço 59.99 e gênero "Action"
    Então o jogo deve ser criado com status "Active" e os dados devem estar corretos

  Esquema do Cenário: Validar falhas no cadastro por dados inválidos
    Quando eu tentar cadastrar um jogo com "<Campo>" inválido: "<Valor>"
    Então deve ser lançada a exceção "<Excecao>"

    Exemplos:
      | Campo     | Valor      | Excecao                        |
      | Titulo    |            | InvalidGameTitleException      |
      | Preco     | -1.00      | InvalidPriceException          |
      | Lancamento| 2000-01-01 | InvalidReleaseDateException    |

  Cenário: Atualizar preço de um jogo existente
    Dado que existe um jogo cadastrado com preço 10.00
    Quando eu atualizar o preço do jogo para 49.99
    Então o preço atual deve ser 49.99
    E a data de atualização deve ser preenchida