# language: pt-br
Funcionalidade: Validação do Objeto de Valor de Nome

  Esquema do Cenário: Validar restrições de nome inválido
    Quando eu tentar criar um nome com o valor <valor>
    Então deve ser lançada uma exceção de domínio de usuário com a mensagem contendo <mensagem>

    Exemplos:
      | valor   | mensagem                      |
      | ""      | "cannot be null or empty"     |
      | "   "   | "cannot be null or empty"     |
      | " a "   | "between 2 and 150"           |
      | null    | "cannot be null or empty"     |