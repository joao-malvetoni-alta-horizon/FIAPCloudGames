# language: pt-br
Funcionalidade: Registro de Jogos na Biblioteca do Usuário

  Cenário: Registrar a aquisição de um jogo com sucesso
    Quando eu registrar a compra de um jogo para o usuário com preço 59.99
    Então o registro deve conter o ID do usuário e do jogo
    E o preço pago deve ser 59.99
    E a data de aquisição deve estar no formato UTC

  Esquema do Cenário: Validar restrições ao registrar aquisição
    Quando eu tentar registrar uma aquisição com "<Campo>" inválido
    Então deve ser lançada uma exceção de domínio de usuário com a mensagem "<Mensagem>"

    Exemplos:
      | Campo    | Mensagem                        |
      | UserId   | UserId cannot be an empty Guid  |
      | GameId   | GameId cannot be an empty Guid  |
      | Preco    | PricePaid cannot be negative    |