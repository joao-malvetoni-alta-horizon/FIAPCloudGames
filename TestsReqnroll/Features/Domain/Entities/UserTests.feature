# language: pt-br
Funcionalidade: Gerenciamento de Usuários

  Cenário: Criar um usuário comum com sucesso
    Quando eu criar um usuário com nome "John Doe", e-mail "user@example.com" e cargo "User"
    Então o usuário deve estar ativo e com os dados preenchidos corretamente

  Esquema do Cenário: Validar restrições na criação de usuário
    Quando eu tentar criar um usuário com "<Campo>" inválido: "<Valor>"
    Então deve ser lançada uma exceção de domínio de usuário contendo "<Mensagem>"

    Exemplos:
      | Campo   | Valor       | Mensagem                     |
      | Nome    |             | name cannot be null or empty |
      | Nome    |    	      | name cannot be null or empty |
      | Email   | notanemail  | invalid format               |
      | RoleId  | empty       | RoleId cannot be an empty Guid|

  Cenário: Gerenciar estado de ativação do usuário
    Dado que existe um usuário "John Doe" cadastrado
    Quando eu desativar o usuário
    Então o usuário deve estar inativo
    Quando eu ativar o usuário
    Então o usuário deve estar ativo novamente

  Cenário: Realizar exclusão lógica (Soft Delete)
    Dado que existe um usuário "John Doe" cadastrado
    Quando eu realizar o soft delete do usuário
    Então o usuário deve estar inativo
    E a data de exclusão deve ser registrada

  Cenário: Alterar cargo do usuário
    Dado que existe um usuário "John Doe" cadastrado com cargo "User"
    Quando eu alterar o cargo para "Administrator"
    Então o cargo do usuário deve ser atualizado com sucesso

  Cenário: Criar administrador raiz (Root Admin)
    Quando eu criar o administrador raiz do sistema
    Então o ID do usuário deve ser o ID padrão de semente do sistema