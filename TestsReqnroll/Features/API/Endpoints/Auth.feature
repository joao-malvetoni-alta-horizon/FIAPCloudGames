# language: pt-br
Funcionalidade: Endpoints de Autenticação

  Cenário: Login com credenciais válidas deve retornar Token
    Dado que o caso de uso de login está configurado para retornar um token válido
    Quando eu realizar uma requisição POST para "/api/auth/login" com e-mail "user@fcg.com" e senha "Valid@123"
    Então o status code da resposta de auth deve ser 200
    E o corpo da resposta deve conter o AccessToken "jwt-token"
    E o tipo do token deve ser "Bearer"
    E o tempo de expiração deve ser 14400

  Cenário: Login com credenciais inválidas deve retornar Unauthorized
    Dado que o caso de uso de login está configurado para lançar erro de credenciais inválidas
    Quando eu realizar uma requisição POST para "/api/auth/login" com e-mail "user@fcg.com" e senha "wrong"
    Então o status code da resposta de auth deve ser 401