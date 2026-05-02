# language: pt-br
Funcionalidade: Autenticação de Usuários

  Cenário: Login falha quando o usuário não é encontrado
    Dado que não existe um usuário cadastrado com o e-mail "unknown@test.com"
    Quando eu tentar realizar o login com e-mail "unknown@test.com" e senha "anypass"
    Então deve ser lançada uma exceção de credenciais inválidas
    E o serviço de token JWT não deve ser chamado

  Cenário: Login falha quando o usuário está inativo
    Dado que existe um usuário cadastrado com o e-mail "inactive@fcg.com"
    Mas esse usuário está inativo
    Quando eu tentar realizar o login com e-mail "inactive@fcg.com" e senha "anypass"
    Então deve ser lançada uma exceção de credenciais inválidas
    E o serviço de token JWT não deve ser chamado

  Cenário: Login falha quando a senha está incorreta
    Dado que existe um usuário ativo cadastrado com e-mail "test@fcg.com" e senha "correct@123"
    Quando eu tentar realizar o login com e-mail "test@fcg.com" e senha "wrongpass"
    Então deve ser lançada uma exceção de credenciais inválidas
    E o serviço de token JWT não deve ser chamado

  Cenário: Login realizado com sucesso
    Dado que existe um usuário ativo cadastrado com e-mail "test@fcg.com" e senha "correct@123"
    Quando eu tentar realizar o login com e-mail "test@fcg.com" e senha "correct@123"
    Então o token de acesso retornado deve ser "jwt-token"
    E o tipo do token deve ser "Bearer"
    E o tempo de expiração deve ser de 14400 segundos
    E o serviço de token JWT deve ser chamado uma única vez