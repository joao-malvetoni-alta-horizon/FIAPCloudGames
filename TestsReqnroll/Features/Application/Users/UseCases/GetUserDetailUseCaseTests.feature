# language: pt-br
Funcionalidade: Obter Detalhes do Usuário

  Cenário: Tentativa de visualizar detalhes de um usuário inexistente
    Dado que um usuário com ID específico não existe no sistema
    Quando eu solicitar os detalhes deste usuário
    Então deve ser lançada uma exceção de usuário não encontrado

  Cenário: Visualizar detalhes de um usuário sem jogos
    Dado que existe um usuário chamado "Detail User" com e-mail "detail@example.com"
    E este usuário não possui jogos em sua biblioteca
    Quando eu solicitar os detalhes deste usuário
    Então os dados retornados devem conter o nome "Detail User" e o e-mail "detail@example.com"
    E a lista de jogos adquiridos deve estar vazia

  Cenário: Retornar nome do cargo vazio quando não houver cargo associado
    Dado que existe um usuário cadastrado no sistema
    E a entidade de cargo (Role) não foi carregada para este usuário
    Quando eu solicitar os detalhes deste usuário
    Então o campo de cargo na resposta deve estar vazio

  Cenário: Validar ordenação decrescente dos jogos adquiridos
    Dado que existe um usuário chamado "Gamer" com jogos em sua biblioteca
    E os jogos foram adquiridos em momentos diferentes
    Quando eu solicitar os detalhes deste usuário
    Então a lista de jogos deve ser retornada em ordem decrescente pela data de aquisição
    E a quantidade total de jogos deve ser 2