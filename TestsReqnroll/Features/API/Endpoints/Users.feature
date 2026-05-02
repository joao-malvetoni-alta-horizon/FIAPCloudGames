# language: pt-br
Funcionalidade: Registro de Usuários

  Cenário: Registro realizado com sucesso
    Dado que o serviço de usuário valida que o e-mail "user@example.com" é único
    E o hasher de senha está configurado
    Quando eu enviar uma requisição POST para "/api/users/register" com nome "User Name", e-mail "user@example.com" e senha "Strong@123"
    Então o status code da resposta de registro deve ser 201
    E o corpo da resposta deve conter o e-mail "user@example.com"

  Cenário: Registro negado por e-mail já existente
    Dado que o serviço de usuário informa que o e-mail "user@example.com" já existe
    E o hasher de senha está configurado
    Quando eu enviar uma requisição POST para "/api/users/register" com nome "User Name", e-mail "user@example.com" e senha "Strong@123"
    Então o status code da resposta de registro deve ser 409

  Cenário: Registro negado por senha inválida
    Dado que o serviço de usuário valida que o e-mail "user@example.com" é único
    E o hasher de senha está configurado
    Quando eu enviar uma requisição POST para "/api/users/register" com nome "User Name", e-mail "user@example.com" e senha "abc"
    Então o status code da resposta de registro deve ser 400