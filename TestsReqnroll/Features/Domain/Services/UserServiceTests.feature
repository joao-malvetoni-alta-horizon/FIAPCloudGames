# language: pt-br
Funcionalidade: Validação de Registro de Usuário

  Cenário: Impedir cadastro com e-mail duplicado
    Dado que o e-mail "existing.user@example.com" já está cadastrado no sistema
    Quando eu validar a unicidade do e-mail "existing.user@example.com"
    Então deve ser lançada uma exceção informando que o usuário já existe com a mensagem contendo "existing.user@example.com"