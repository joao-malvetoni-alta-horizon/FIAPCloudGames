# language: pt-br
Funcionalidade: Listagem Paginada de Usuários

  Cenário: Listar usuários com parâmetros válidos
    Dado que existem usuários cadastrados no sistema:
      | Nome  | Email              |
      | Alice | alice@example.com  |
      | Bob   | bob@example.com    |
    Quando eu solicitar a listagem na página 1 com tamanho 10
    Então a resposta deve conter 2 itens
    E o total de registros deve ser 2
    E a página atual deve ser 1
    E o tamanho da página deve ser 10
    E a lista deve conter o e-mail "alice@example.com"

  Esquema do Cenário: Validar sanitização de parâmetros de paginação
    Dado que o sistema possui usuários cadastrados
    Quando eu solicitar a listagem na página <inputPage> com tamanho <inputSize>
    Então a página resultante deve ser <expectedPage>
    E o tamanho da página resultante deve ser <expectedSize>
    E o repositório deve ser consultado com <expectedPage> e <expectedSize>

    Exemplos:
      | inputPage | inputSize | expectedPage | expectedSize |
      | 0         | 10        | 1            | 10           |
      | -5        | 10        | 1            | 10           |
      | 1         | 0         | 1            | 10           |
      | 1         | -3        | 1            | 10           |
      | 1         | 50        | 1            | 10           |

  Cenário: Listagem sem usuários cadastrados
    Dado que não existem usuários no sistema
    Quando eu solicitar a listagem na página 1 com tamanho 10
    Então a lista de itens deve vir vazia
    E o total de registros deve ser 0

  Cenário: Usuário na lista sem cargo associado
    Dado que existe um usuário chamado "No Role" sem cargo carregado
    Quando eu solicitar a listagem na página 1 com tamanho 10
    Então o nome do cargo para este usuário deve ser vazio