# language: pt-br
Funcionalidade: Atualização de Promoção de Jogos

  Contexto:
    Dado que o ID do cargo de administrador é definido
    E que o ID do cargo de usuário comum é definido

  Cenário: Tentativa de atualização por usuário sem permissão
    Dado que existe uma promoção cadastrada no sistema
    Quando um usuário comum tentar atualizar essa promoção para 30% de desconto
    Então deve ser lançada uma exceção de permissão insuficiente
    E as alterações não devem ser persistidas no banco

  Cenário: Tentativa de atualização de promoção inexistente
    Dado que uma promoção com ID específico não existe no sistema
    Quando um administrador tentar atualizar uma promoção inexistente para 30% de desconto
    Então deve ser lançada uma exceção de promoção não encontrada

  Cenário: Tentativa de atualização com sobreposição de datas
    Dado que existe uma promoção cadastrada no sistema
    E existe uma outra promoção ativa que sobrepõe o novo período
    Quando um administrador tentar atualizar essa promoção estendendo o período
    Então deve ser lançada uma exceção de sobreposição de promoção
    E as alterações não devem ser persistidas no banco

  Cenário: Atualização de promoção com sucesso
    Dado que existe uma promoção cadastrada no sistema
    E não existem promoções sobrepostas para o novo período
    Quando um administrador tentar atualizar essa promoção para um valor fixo de 5.99
    Então a promoção deve ser atualizada com sucesso
    E os detalhes da promoção devem refletir o novo valor de 5.99
    E as alterações devem ser persistidas no banco uma única vez

  Cenário: Desativação de promoção sem validar sobreposição
    Dado que existe uma promoção cadastrada no sistema
    Quando um administrador solicitar a desativação da promoção
    Então a promoção deve ser desativada com sucesso
    E o sistema não deve validar a sobreposição de promoções
    E as alterações devem ser persistidas no banco uma única vez