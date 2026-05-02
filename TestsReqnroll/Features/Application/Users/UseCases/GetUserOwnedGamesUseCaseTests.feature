# language: pt-br
Funcionalidade: Obter Jogos Adquiridos pelo Usuário

  Contexto:
    Dado que o Unit of Work fornece acesso aos repositórios de usuários e biblioteca

  Cenário: Tentativa de obter jogos de um usuário inexistente
    Dado que o ID do usuário informado não corresponde a nenhum registro
    Quando eu solicitar a lista de jogos deste usuário
    Então deve ser lançada uma exceção de usuário não encontrado

  Cenário: Obter lista de jogos com sucesso e ordenacao decrescente
    Dado que existe um usuário ativo cadastrado no sistema
    E que este usuário adquiriu os seguintes jogos:
      | Preco |
      | 20.00 |
      | 35.00 |
    Quando eu solicitar a lista de jogos deste usuário
    Então a resposta deve conter 2 jogos
    E o primeiro jogo da lista deve ser o mais recente com preço 35.00
    E o segundo jogo da lista deve ser o mais antigo com preço 20.00

  Cenário: Retornar lista vazia quando o usuário não possui jogos
    Dado que existe um usuário ativo cadastrado no sistema
    E o usuário ainda não adquiriu nenhum jogo
    Quando eu solicitar a lista de jogos deste usuário
    Então a resposta deve ser uma lista vazia