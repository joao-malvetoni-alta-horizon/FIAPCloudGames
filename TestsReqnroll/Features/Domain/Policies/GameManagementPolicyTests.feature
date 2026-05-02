# language: pt-br
Funcionalidade: Política de Gerenciamento de Jogos

  Como sistema de segurança
  Eu quero validar as permissões de acesso
  Para garantir que apenas administradores gerenciem o catálogo de jogos

  Esquema do Cenário: Verificar permissão por perfil de usuário
    Quando eu verificar a permissão de gerenciamento para o perfil "<Perfil>"
    Então o sistema deve permitir a ação: "<Permitido>"

    Exemplos:
      | Perfil        | Permitido |
      | Administrator | True      |
      | User          | False     |
      | Desconhecido  | False     |