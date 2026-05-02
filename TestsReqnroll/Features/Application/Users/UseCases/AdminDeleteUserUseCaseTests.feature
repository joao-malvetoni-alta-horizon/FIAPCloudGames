# language: pt-br
Funcionalidade: Exclusão de Usuário por Administrador

  Contexto:
    Dado que o repositório de usuários está configurado no Unit of Work

  Cenário: Tentativa de exclusão de usuário inexistente
    Dado que um usuário com ID específico não existe no sistema
    Quando o administrador solicitar a exclusão deste usuário
    Então deve ser lançada uma exceção de usuário não encontrado
    E as alterações de usuário não devem ser persistidas

  Cenário: Impedir a exclusão do Administrador Raiz (Root)
    Dado que o usuário alvo é o Administrador Raiz do sistema
    Quando o administrador solicitar a exclusão deste usuário
    Então deve ser lançada uma exceção de operação proibida para o Root Admin
    E o status do usuário não deve ser alterado no banco

  Cenário: Exclusão lógica (Soft Delete) de usuário comum com sucesso
    Dado que existe um usuário comum cadastrado no sistema
    Quando o administrador solicitar a exclusão deste usuário
    Então o usuário deve ser marcado como inativo
    E a data de exclusão deve ser registrada
    E as alterações de usuário devem ser persistidas uma única vez