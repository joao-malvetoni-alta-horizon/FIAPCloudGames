# language: pt-br
Funcionalidade: Validação de Objeto de Valor de E-mail

  Cenário: Criar e-mail válido
    Quando eu criar um e-mail com o endereço "user@example.com"
    Então o endereço deve ser armazenado como "user@example.com"

  Cenário: Criar e-mail com subdomínio
    Quando eu criar um e-mail com o endereço "user@mail.example.com"
    Então o endereço deve ser armazenado como "user@mail.example.com"

  Esquema do Cenário: Validar formatos de e-mail inválidos
    Quando eu tentar criar um e-mail com o valor "<Valor>"
    Então deve ser lançada uma exceção de domínio de usuário com a mensagem "<Mensagem>"

    Exemplos:
      | Valor              | Mensagem        |
      |                    | null or empty   |
      | userexample.com    | invalid format  |
      | user@              | invalid format  |
      | user @example.com  | invalid format  |

  Cenário: Comparar e-mails idênticos (Case Insensitivity)
    Dado que eu tenho o e-mail "user@example.com"
    E eu tenho outro e-mail "USER@EXAMPLE.COM"
    Então os dois e-mails devem ser considerados iguais

  Cenário: Conversão implícita entre string e e-mail
    Quando eu converter a string "user@example.com" implicitamente para E-mail
    Então o resultado deve ser um objeto Email com o endereço correto