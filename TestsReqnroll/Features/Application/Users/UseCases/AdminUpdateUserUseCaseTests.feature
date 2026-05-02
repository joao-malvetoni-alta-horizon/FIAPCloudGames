# language: pt-br
Funcionalidade: Atualização de Usuário por Administrador

  Contexto:
    Dado que o Unit of Work está configurado com repositórios de usuários e cargos

  Cenário: Tentativa de atualizar usuário inexistente
    Dado que um usuário com ID específico não existe no sistema
    Quando o administrador tentar atualizar os dados deste usuário
    Então deve ser lançada uma exceção de usuário não encontrado

  Cenário: Impedir a atualização do Administrador Raiz (Root)
    Dado que o usuário alvo é o Administrador Raiz do sistema
    Quando o administrador tentar desativar o Administrador Raiz
    Então deve ser lançada uma exceção de operação proibida para o Root Admin
    E as alterações não devem ser persistidas no banco

  Cenário: Tentativa de atribuir um cargo inexistente
    Dado que existe um usuário cadastrado no sistema
    E o cargo com ID informado não existe
    Quando o administrador tentar mudar o cargo do usuário
    Então deve ser lançada uma exceção de domínio de usuário
    E as alterações não devem ser persistidas no banco

  Cenário: Desativar um usuário com sucesso
    Dado que existe um usuário ativo no sistema
    Quando o administrador solicitar a desativação do usuário
    Então o usuário deve ser marcado como inativo
    E as alterações devem ser persistidas no banco uma única vez

  Cenário: Alterar o cargo de um usuário com sucesso
    Dado que existe um usuário cadastrado no sistema
    E o cargo de "Administrador" existe no sistema
    Quando o administrador solicitar a mudança de cargo do usuário
    Então o usuário deve passar a ter o novo cargo
    E as alterações devem ser persistidas no banco uma única vez

  Cenário: Atualização com campos nulos não deve alterar o usuário
    Dado que existe um usuário cadastrado no sistema
    Quando o administrador enviar uma atualização com campos nulos
    Então o estado do usuário deve permanecer o mesmo
    E o sistema não deve buscar informações de cargo no banco
    E as alterações devem ser persistidas no banco uma única vez