# language: pt-br
Funcionalidade: Validação de Complexidade de Senha

  Como sistema de segurança
  Eu quero validar os requisitos de complexidade da senha
  Para garantir que os usuários utilizem credenciais fortes

  Cenário: Validar senha forte com sucesso
    Quando eu validar a senha "StrongPass1!"
    Então o sistema não deve lançar nenhuma exceção

  Esquema do Cenário: Validar falhas de requisitos de senha
    Quando eu tentar validar a senha "<Senha>"
    Então deve ser lançada uma exceção de domínio de usuário com a mensagem "<Mensagem>"

    Exemplos:
      | Senha        | Mensagem              |
      | Ab1!         | at least 8 characters |
      | lowercase1!  | uppercase             |
      | UPPERCASE1!  | lowercase             |
      | NoDigitPass! | digit                 |
      | NoSpecial1A  | special character     |
      |              | null or empty         |