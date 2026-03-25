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
