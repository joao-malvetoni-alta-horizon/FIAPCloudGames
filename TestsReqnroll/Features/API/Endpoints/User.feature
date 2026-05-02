# language: pt-br
Funcionalidade: User Endpoints

  Cenário: PostPurchaseOwnedGame quando autenticado deve passar UserId do JWT para o UseCase
    Dado que o usuário está autenticado
    Quando eu realizar uma requisição POST para "/api/users/owned-games"
    Então o status code da resposta deve ser 201
    E o UseCase deve ser chamado uma única vez com o UserId correto

  Cenário: PostPurchaseOwnedGame quando não autenticado deve retornar Unauthorized
    Dado que o usuário não está autenticado
    Quando eu realizar uma requisição POST para "/api/users/owned-games"
    Então o status code da resposta deve ser 401