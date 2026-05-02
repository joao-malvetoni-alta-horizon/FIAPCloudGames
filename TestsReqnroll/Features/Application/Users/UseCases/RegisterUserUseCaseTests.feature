# language: pt-br
Funcionalidade: Registro de Novo Usuário

  Contexto:
    Dado que o sistema de hashing de senhas está ativo
    E o serviço de domínio de usuários está disponível

  Cenário: Registro bem-sucedido de novo usuário
    Dado que o e-mail "new@example.com" ainda não está cadastrado
    Quando eu solicitar o registro com o nome "New User", e-mail "new@example.com" e senha "Strong@123"
    Então o sistema deve realizar o hash da senha
    E os dados do novo usuário devem ser salvos e persistidos
    E a resposta deve conter o e-mail "new@example.com" e o nome "New User"
    E o cargo padrão deve ser do tipo "User"

  Cenário: Tentativa de registro com e-mail já existente
    Dado que o e-mail "existing@example.com" já está cadastrado no sistema
    Quando eu solicitar o registro com o nome "Existing User", e-mail "existing@example.com" e senha "Valid@123"
    Então deve ser lançada uma exceção informando que o usuário já existe

  Cenário: Tentativa de registro com senha fraca
    Dado que o e-mail "weak@example.com" está disponível
    Quando eu solicitar o registro com o nome "Weak Password User", e-mail "weak@example.com" e senha "abc"
    Então deve ser lançada uma exceção de domínio de usuário
    E o sistema não deve realizar o hash da senha nem persistir os dados