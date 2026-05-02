# language: pt-br
Funcionalidade: Criação de Promoção de Jogos

  Contexto:
    Dado que o ID do cargo de administrador é definido
    E que o ID do cargo de usuário comum é definido

  Cenário: Tentativa de criação por usuário sem permissão
    Dado que existe um jogo cadastrado para promoção
    Quando um usuário comum tentar criar uma promoção de 20% de desconto
    Então deve ser lançada uma exceção de permissão insuficiente
    E as alterações não devem ser persistidas no banco

  Cenário: Tentativa de criação para jogo inexistente
    Dado que um jogo com ID específico não existe no sistema
    Quando um administrador tentar criar uma promoção para este ID inexistente
    Então deve ser lançada uma exceção de jogo não encontrado
    E as alterações não devem ser persistidas no banco

  Cenário: Tentativa de criação com promoção sobreposta
    Dado que existe um jogo cadastrado para promoção
    E já existe uma promoção ativa para este jogo no mesmo período
    Quando um administrador tentar criar uma promoção de 20% de desconto
    Então deve ser lançada uma exceção de sobreposição de promoção
    E as alterações não devem ser persistidas no banco

  Cenário: Criação de promoção com sucesso
    Dado que existe um jogo cadastrado para promoção
    E não existem promoções sobrepostas para este jogo no período
    Quando um administrador tentar criar uma promoção de 20% de desconto
    Então a promoção deve ser criada com sucesso
    E os detalhes da promoção devem refletir o desconto de 20%
    E as alterações devem ser persistidas no banco uma única vez