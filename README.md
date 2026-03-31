# FIAP Cloud Games (FCG)

Plataforma de gerenciamento de jogos na nuvem, desenvolvida com .NET 10 seguindo Domain-Driven Design (DDD) em arquitetura monolítica.

## Estrutura do Projeto

```
src/
  FCG.Domain/          → Entidades, Value Objects, Enums, Eventos, Exceções
  FCG.Application/     → Casos de Uso, DTOs, Interfaces
  FCG.Infrastructure/  → EF Core, Repositórios, Mapeamentos, DI
  FCG.API/             → Endpoints REST (Minimal API), Middlewares, Swagger
```

## Pré-requisitos

- [Docker](https://docs.docker.com/get-docker/) e [Docker Compose](https://docs.docker.com/compose/install/)

## Como rodar

```bash
docker-compose up --build
```

A API estará disponível em: **http://localhost:8080**

Swagger UI: **http://localhost:8080/swagger**

## Endpoints

| Método | Rota               | Descrição                        |
|--------|--------------------|---------------------------------|
| POST   | /api/games         | Criar um novo jogo              |
| GET    | /api/games/{id}    | Buscar jogo por ID              |
| GET    | /api/games         | Listar jogos (paginado)         |
| PUT    | /api/games/{id}    | Atualizar um jogo               |
| DELETE | /api/games/{id}    | Desativar um jogo (soft delete) |

### Parâmetros de query para listagem

- `page` (int, default: 1)
- `pageSize` (int, default: 10, máx: 50)
- `genre` (opcional: Action, RPG, Strategy, Sports, Puzzle, Other)

### Exemplo de criação de jogo

```json
POST /api/games
{
  "title": "Elden Ring",
  "description": "Action RPG by FromSoftware",
  "price": 199.90,
  "genre": "RPG",
  "releaseDate": "2026-06-01"
}
```

## Tecnologias

- .NET 10 (Minimal APIs)
- Entity Framework Core + PostgreSQL
- Swashbuckle (Swagger)
- Docker / Docker Compose


## Linguagem Ubíqua 

### Entidades e Agregados

| Termo | Definição | Representação no código|
|--------|--------------------|---------------------------------|
| Usuário| Indíviduo apto a consumir ou administrar jogos na plataforma | User |
| Jogo   | Conteúdo interativo digital | Game |
| Biblioteca | Acervo de jogos adquiridos pelo jogador | UserGameLibrary |
| Papel | Define os níveis de autoridade de um usuário | Role |

### Objetos de Valor

| Representação no código | Definição |
|--------|--------------------|
| Name | Nome do usuário |
| Email| Endereço de -mail utilizado no cadastro do usuário |
| Password | Senha de acesso do usuário, deve conter 8 dígitos e ao menos um dos caracteres especiais "!@#$%^&*()-_+="|
| Price | Representa o custo do jogo; nunca pode ser um valor negativo |
| Price | Define os níveis de autoridade de um usuário |

### Dicionário de Estados (Enums)

- RoleType: Define o nível de acesso do User
  * User
  * Administrator

- GameGenre: Define o gênero do Game
  * Action
  * RPG
  * Strategy
  * Sports
  * Puzzle
  * Other
 
- GameStatus: Define o status do Game
  * Active (Ativo)
  * Inactive (Inativo)
  * ComingSoon (Em breve)
  
  
