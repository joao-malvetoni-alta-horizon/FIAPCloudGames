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

> 🔒 Requer autenticação (JWT)

### Auth

| Método | Rota            | Descrição        |
|--------|-----------------|------------------|
| POST   | /api/auth/login | Gerar token JWT  |

### Games

| Método | Rota            | Descrição                        |
|--------|-----------------|---------------------------------|
| POST   | /api/games      | Criar um novo jogo 🔒           |
| GET    | /api/games      | Listar jogos (paginado/filtro)  |
| GET    | /api/games/{id} | Buscar jogo por ID              |
| PUT    | /api/games/{id} | Atualizar um jogo 🔒           |
| DELETE | /api/games/{id} | Desativar jogo 🔒              |

### Promotions

| Método | Rota                                      | Descrição                        |
|--------|-------------------------------------------|---------------------------------|
| GET    | /api/games/{gameId}/promotions            | Listar promoções do jogo         |
| GET    | /api/games/{gameId}/promotions/{id}       | Buscar promoção por ID           |

### Users

| Método | Rota                                  | Descrição                        |
|--------|---------------------------------------|---------------------------------|
| POST   | /api/users/register                   | Registrar novo usuário          |
| GET    | /api/users/{userId}/owned-games       | Listar jogos do usuário 🔒      |
| POST   | /api/users/{userId}/owned-games       | Adicionar jogo ao usuário 🔒    |

### Admin - Users

| Método | Rota                     | Descrição                |
|--------|--------------------------|-------------------------|
| POST   | /api/admin/users         | Criar usuário 🔒        |
| GET    | /api/admin/users         | Listar usuários 🔒      |
| GET    | /api/admin/users/{id}    | Buscar usuário por ID 🔒|
| PUT    | /api/admin/users/{id}    | Atualizar usuário 🔒    |
| DELETE | /api/admin/users/{id}    | Remover usuário 🔒      |

### Admin - Promotions

| Método | Rota                                               | Descrição                |
|--------|----------------------------------------------------|-------------------------|
| POST   | /api/admin/games/{gameId}/promotions              | Criar promoção 🔒       |
| PUT    | /api/admin/games/{gameId}/promotions/{id}         | Atualizar promoção 🔒   |
| DELETE | /api/admin/games/{gameId}/promotions/{id}         | Remover promoção 🔒     |

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

### Observações

- Endpoints marcados com 🔒 requerem autenticação JWT
- Endpoints de **Admin** requerem permissão de administrador
- IDs são do tipo `UUID`
- Exclusões são do tipo *soft delete* (quando aplicável)

## Tecnologias

- .NET 10 (Minimal APIs)
- Entity Framework Core + PostgreSQL
- Swashbuckle (Swagger)
- Docker / Docker Compose

## Documentação

- Requisitos da fase: [TC NETT - Fase 1.pdf](./TC%20NETT%20-%20Fase%201.pdf)


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
| Id | Guid utilizado na criação de qualquer entidade |
| Name | Nome do usuário |
| Email| Endereço de e-mail utilizado no cadastro do usuário |
| Password | Senha de acesso do usuário, deve conter 8 dígitos e ao menos um dos caracteres especiais "!@#$%^&*()-_+="|
| Price | Representa o custo do jogo; nunca pode ser um valor negativo |

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
  
  
