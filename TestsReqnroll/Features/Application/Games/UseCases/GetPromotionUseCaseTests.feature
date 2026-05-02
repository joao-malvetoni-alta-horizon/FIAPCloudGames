# language: pt-br
Funcionalidade: Consulta de Promoção de Jogos

  Cenário: Tentativa de consulta de promoção inexistente
    Dado que uma promoção com ID específico não existe no sistema
    Quando eu buscar pelos detalhes dessa promoção
    Então deve ser lançada uma exceção de promoção não encontrada

  Cenário: Consulta de promoção com sucesso
    Dado que existe uma promoção cadastrada no sistema
    Quando eu buscar pelos detalhes dessa promoção
    Então os detalhes da promoção devem ser retornados corretamente
    E o status da promoção deve ser ativo