# language: pt-br
Funcionalidade: Compra de Jogo pelo Usuário

  Contexto:
    Dado que o sistema de persistência (UoW) está operacional
    E os repositórios de usuários, jogos e biblioteca estão configurados

  Cenário: Compra realizada com sucesso
    Dado que existe um usuário ativo no sistema
    E um jogo disponível com status "Active" e preço 120.00
    E o usuário ainda não possui este jogo na biblioteca
    Quando o usuário solicitar a compra do jogo
    Então a compra deve ser registrada com o preço de 120.00
    E a transação deve ser persistida no banco de dados

  Cenário: Tentativa de compra por usuário inexistente
    Dado que o ID do usuário informado não corresponde a nenhum registro
    Quando o usuário solicitar a compra do jogo
    Então deve ser lançada uma exceção de usuário não encontrado

  Cenário: Usuário inativo não pode comprar jogos
    Dado que existe um usuário inativo no sistema
    Quando o usuário solicitar a compra do jogo
    Então deve ser lançada uma exceção de domínio com a mensagem "Inactive users cannot acquire games."

  Cenário: Impedir compra de jogos que não estão ativos
    Dado que existe um usuário ativo no sistema
    E um jogo disponível com status "ComingSoon"
    Quando o usuário solicitar a compra do jogo
    Então deve ser lançada uma exceção de validação com a mensagem "Only active games can be acquired."

  Cenário: Impedir compra de jogo já adquirido (duplicidade)
    Dado que existe um usuário ativo no sistema
    E um jogo disponível com status "Active"
    E o usuário já possui este jogo em sua biblioteca
    Quando o usuário solicitar a compra do jogo
    Então deve ser lançada uma exceção informando que o usuário já possui o jogo