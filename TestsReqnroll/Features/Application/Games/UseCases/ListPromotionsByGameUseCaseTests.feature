# language: pt-br
Funcionalidade: Listagem de Promoções por Jogo

  Cenário: Listar promoções de um jogo que não possui ofertas
    Dado que um jogo com ID específico não possui promoções cadastradas
    Quando eu solicitar a lista de promoções desse jogo
    Então a lista retornada deve estar vazia

  Cenário: Listar promoções de um jogo com sucesso
    Dado que existem promoções cadastradas para um jogo específico
    Quando eu solicitar a lista de promoções desse jogo
    Então a lista deve conter exatamente "2" promoções
    E todas as promoções devem pertencer ao jogo solicitado

  Cenário: Garantir ordenação das promoções
    Dado que existem promoções com diferentes datas de início para um jogo
    Quando eu solicitar a lista de promoções desse jogo
    Então a promoção mais recente deve aparecer primeiro na lista