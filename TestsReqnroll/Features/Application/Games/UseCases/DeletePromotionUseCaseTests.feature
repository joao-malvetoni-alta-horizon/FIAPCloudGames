# language: pt-br
Funcionalidade: Exclusão de Promoção de Jogos

  Contexto:
    Dado que o ID do cargo de administrador é definido
    E que o ID do cargo de usuário comum é definido

  Cenário: Tentativa de exclusão por usuário sem permissão
    Quando um usuário comum tentar excluir uma promoção existente
    Então deve ser lançada uma exceção de permissão insuficiente
    E as alterações não devem ser persistidas no banco

  Cenário: Tentativa de exclusão de promoção inexistente
    Dado que uma promoção com ID específico não existe no sistema
    Quando um administrador tentar excluir esta promoção inexistente
    Então deve ser lançada uma exceção de promoção não encontrada
    E as alterações não devem ser persistidas no banco

  Cenário: Exclusão de promoção com sucesso
    Dado que existe uma promoção cadastrada no sistema
    Quando um administrador tentar excluir esta promoção existente
    Então a promoção deve ser removida com sucesso
    E as alterações devem ser persistidas no banco uma única vez