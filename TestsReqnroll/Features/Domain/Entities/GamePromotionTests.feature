# language: pt-br
Funcionalidade: Gerenciamento de Promoções de Jogos

  Cenário: Criar uma promoção válida
    Quando eu criar uma promoção para um jogo com desconto de "Percentage" de 20
    E a data de início for amanhã e o término em 10 dias
    Então a promoção deve ser criada com sucesso e estar ativa

  Esquema do Cenário: Validar valores de desconto inválidos
    Quando eu tentar criar uma promoção com desconto de "<Tipo>" de <Valor>
    Então deve ser lançada uma exceção de validação de domínio com a mensagem "<Mensagem>"

    Exemplos:
      | Tipo       | Valor | Mensagem         |
      | FixedValue | 0     | greater than zero |
      | FixedValue | -5    | greater than zero |
      | Percentage | 101   | 100%             |

  Cenário: Impedir data de início posterior ou igual à data de término
    Quando eu tentar criar uma promoção onde a data de início é após o término
    Então deve ser lançada uma exceção de validação com a mensagem "before end date"

  Esquema do Cenário: Verificar validade temporal da promoção
    Dado que existe uma promoção com início em "<Inicio>" dias e término em "<Termino>" dias
    Quando eu verificar se a promoção é válida atualmente
    Então o resultado deve ser "<Resultado>"

    Exemplos:
      | Inicio | Termino | Resultado | Status      |
      | -1     | 1       | True      | No Prazo    |
      | -10    | -1      | False     | Expirada    |
      | 5      | 10      | False     | Futura      |

  Cenário: Desativar uma promoção manualmente
    Dado que existe uma promoção ativa
    Quando eu desativar a promoção
    Então a promoção não deve mais ser considerada válida atualmente
    E o campo IsActive deve ser falso