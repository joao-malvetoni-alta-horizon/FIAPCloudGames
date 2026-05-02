# language: pt-br
Funcionalidade: Geração de Token JWT

  Como o sistema de segurança FCG
  Eu quero gerar tokens de acesso JWT para autenticar usuários com base em seus cargos

  Cenário: Verificar formato e claims para usuário comum
    Dado que eu tenho um usuário com o cargo "User"
    E a configuração de expiração é de 1 hora
    Quando eu gerar o token de autenticação
    Então o token deve retornar um formato JWT válido
    E o payload deve conter o nome do cargo "User"
    E o payload deve conter o ID do usuário
    E o payload deve conter o e-mail do usuário
    E o payload deve conter a claim "roleId" com o valor "1"
    E a expiração deve ser definida com base nas configurações

  Cenário: Verificar claims para administrador
    Dado que eu tenho um usuário com o cargo "Administrator"
    Quando eu gerar o token de autenticação
    Então o payload deve conter o nome do cargo "Administrator"
    E o payload deve conter o ID do cargo de administrador correspondente a "2"