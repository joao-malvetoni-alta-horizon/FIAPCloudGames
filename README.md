# FIAP Cloud Games (FCG)

Plataforma de venda de jogos digitais e gestão de biblioteca de jogos, desenvolvida como MVP da Fase 1 do Tech Challenge FIAP. Construída com .NET 10 seguindo Domain-Driven Design (DDD) em arquitetura monolítica.

## Objetivo

Criar uma API REST para gerenciar usuários e seus jogos adquiridos, com autenticação JWT, autorização por papéis (Usuário/Administrador), persistência com Entity Framework Core e qualidade assegurada por testes unitários e BDD.

## Tecnologias

- .NET 10 (Minimal APIs)
- Entity Framework Core + PostgreSQL
- JWT (autenticação e autorização)
- BCrypt (hash de senhas)
- Serilog (logs estruturados)
- Swagger / OpenAPI
- xUnit + Moq + FluentAssertions (testes unitários)
- Reqnroll + Gherkin (testes BDD)
- Docker / Docker Compose

## Estrutura do Projeto

```
src/
  FCG.Domain/          → Entidades, Value Objects, Enums, Eventos, Exceções, Políticas
  FCG.Application/     → Casos de Uso, DTOs, Interfaces, Mappers
  FCG.Infrastructure/  → EF Core, Repositórios, JWT, BCrypt, DI
  FCG.API/             → Endpoints REST (Minimal API), Middlewares, Swagger
  FCG.Tests/           → Testes Unitários e BDD
```

## Pré-requisitos

- [Docker](https://docs.docker.com/get-docker/) e [Docker Compose](https://docs.docker.com/compose/install/)
- (Opcional) [.NET 10 SDK](https://dotnet.microsoft.com/download) para desenvolvimento local

## Como executar

### Via Docker (recomendado)

```bash
docker-compose up --build
```

A API estará disponível em **http://localhost:8080** e o Swagger em **http://localhost:8080/swagger**.

### Localmente

```bash
# Subir apenas o banco
docker-compose up db -d

# Rodar a API
cd src/FCG.API
dotnet run
```

## Credenciais do Administrador (seed)

Ao iniciar, a aplicação cria automaticamente um usuário administrador:

| Campo | Valor |
|-------|-------|
| E-mail | `admin@fcg.com` |
| Senha | `Admin@123` |

Use essas credenciais no endpoint `/api/auth/login` para obter o token JWT.

## Endpoints

> 🔒 = Requer token JWT | 🔑 = Requer papel Administrador

### Autenticação

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/auth/login` | Gerar token JWT | — |

### Usuários

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/users/register` | Cadastrar novo usuário | — |
| GET | `/api/users/{userId}/owned-games` | Listar jogos do usuário | 🔒 |
| POST | `/api/users/{userId}/owned-games` | Adquirir jogo | 🔒 |

### Jogos

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/games` | Cadastrar jogo | 🔑 |
| GET | `/api/games` | Listar jogos (paginado) | 🔒 |
| GET | `/api/games/{id}` | Buscar jogo por ID | 🔒 |
| PUT | `/api/games/{id}` | Atualizar jogo | 🔑 |
| DELETE | `/api/games/{id}` | Desativar jogo (soft delete) | 🔑 |

### Promoções

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| GET | `/api/games/{gameId}/promotions` | Listar promoções do jogo | 🔒 |
| GET | `/api/games/{gameId}/promotions/{id}` | Buscar promoção | 🔒 |
| POST | `/api/admin/games/{gameId}/promotions` | Criar promoção | 🔑 |
| PUT | `/api/admin/games/{gameId}/promotions/{id}` | Atualizar promoção | 🔑 |
| DELETE | `/api/admin/games/{gameId}/promotions/{id}` | Remover promoção | 🔑 |

### Admin — Usuários

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/admin/users` | Criar usuário | 🔑 |
| GET | `/api/admin/users` | Listar usuários | 🔑 |
| GET | `/api/admin/users/{id}` | Buscar usuário | 🔑 |
| PUT | `/api/admin/users/{id}` | Atualizar usuário | 🔑 |
| DELETE | `/api/admin/users/{id}` | Remover usuário (soft delete) | 🔑 |

### Parâmetros de listagem

| Parâmetro | Tipo | Padrão | Descrição |
|-----------|------|--------|-----------|
| `page` | int | 1 | Página |
| `pageSize` | int | 10 | Itens por página (máx. 50) |
| `genre` | string | — | Filtro por gênero (Action, RPG, Strategy, Sports, Puzzle, Other) |

## Validações

- **E-mail**: formato RFC 5322, único no sistema
- **Senha**: mínimo 8 caracteres, pelo menos 1 maiúscula, 1 minúscula, 1 dígito e 1 caractere especial (`!@#$%^&*()-_+=`)
- **Preço do jogo**: deve ser positivo
- **Promoções**: sem sobreposição de datas para o mesmo jogo; desconto percentual ≤ 100%

## Como executar os testes

```bash
cd src
dotnet test
```

## Linguagem Ubíqua

### Entidades e Agregados

| Termo | Definição | Código |
|-------|-----------|--------|
| Usuário | Indivíduo apto a consumir ou administrar jogos na plataforma | `User` |
| Jogo | Conteúdo interativo digital disponível para aquisição | `Game` |
| Biblioteca | Acervo de jogos adquiridos pelo jogador | `UserOwnedGame` |
| Papel | Define o nível de autoridade de um usuário | `Role` |
| Promoção | Desconto temporário aplicado a um jogo | `GamePromotion` |

### Objetos de Valor

| Código | Definição |
|--------|-----------|
| `Name` | Nome do usuário |
| `Email` | Endereço de e-mail (normalizado para lowercase) |
| `Password` | Validador de complexidade de senha |
| `GameTitle` | Título do jogo |
| `Price` | Valor monetário do jogo (nunca negativo) |

### Enums

- **RoleType**: `User`, `Administrator`
- **GameGenre**: `Action`, `RPG`, `Strategy`, `Sports`, `Puzzle`, `Other`
- **GameStatus**: `Active`, `Inactive`, `ComingSoon`
- **DiscountType**: `Percentage`, `FixedValue`
