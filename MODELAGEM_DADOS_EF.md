# Documentação: Modelagem de Dados e Entity Framework

> **📹 COMO USAR ESTE DOCUMENTO PARA VÍDEO:**
> 
> Este documento foi criado especialmente para servir como roteiro de apresentação em vídeo. Cada seção tem um box com "📹 O que dizer:" que contém a narração sugerida, em tom informal e conversacional, explicando os conceitos técnicos de forma que qualquer pessoa entenda.
>
> **Dica**: Ao gravar o vídeo, você pode seguir a narração sugerida, mostrando o código e o banco de dados na tela enquanto fala. Os detalhes técnicos estão lá para consulta, enquanto o tom conversacional torna a explicação acessível.

## Índice
1. [Visão Geral da Modelagem de Dados](#-visão-geral-da-modelagem-de-dados)
2. [Visão Geral da Arquitetura](#visão-geral-da-arquitetura)
3. [Padrão de Design DDD](#padrão-de-design-ddd)
4. [Estrutura de Camadas](#estrutura-de-camadas)
5. [Entidades do Domínio](#entidades-do-domínio)
6. [Value Objects](#value-objects)
7. [DbContext e Configurações](#dbcontext-e-configurações)
8. [Migrations](#migrations)
9. [Relacionamentos e Integridade Referencial](#relacionamentos-e-integridade-referencial)
10. [Decisões Arquiteturais](#decisões-arquiteturais)
11. [Índices e Constraints](#índices-e-constraints)

---

## 🎯 VISÃO GERAL DA MODELAGEM DE DADOS

> **📹 O que dizer:**
>
> "Oi! Bem-vindo. Vou falar sobre como a gente modelou os dados do FIAP Cloud Games. Modelagem de dados é como você organiza as informações no seu sistema - é tipo um blueprint, um diagrama de como tudo vai ser guardado e relacionado.
>
> A gente escolheu uma abordagem bem pensada chamada Domain-Driven Design, que significa: a lógica de negócio vem em primeiro lugar, e o banco de dados vem depois.
>
> A ideia é essa: a gente tem um domínio de negócio - que é vender jogos para usuários. A gente modela entidades importantes desse domínio - como Game (um jogo), User (um usuário), Role (permissões). Mas a gente não só tira dados e joga no banco. Não, a gente coloca REGRAS de negócio nas entidades.
>
> Por exemplo: um título de jogo não pode ser vazio, tem que ter no máximo 200 caracteres, e tem que ser único. Um preço não pode ser negativo. Um email tem que ser válido. Um usuário não pode comprar o mesmo jogo duas vezes.
>
> Essas regras não ficam espalhadas em 50 lugares do código. Elas ficam na entidade, no domínio. Isso é seguro, é testável, é fácil de manter.
>
> A gente também usa Value Objects - que são valores imutáveis que encapsulam validação. Tipo GameTitle, Price, Email. Uma vez criados, não mudam, e é impossível criar um inválido.
>
> Tudo isso junto - Entity Framework, Migrations, Índices, Constraints - forma um sistema robusto, seguro e escalável."

### O que é Modelagem de Dados?

**Modelagem de dados** é o processo de definir a estrutura de como os dados serão armazenados, relacionados e acessados. É como desenhar a arquitetura do banco de dados antes de executar uma linha de SQL.

### Abordagem Utilizada no FIAP Cloud Games

```
Domínio de Negócio
    ↓
Entidades & Value Objects (com regras)
    ↓
Entity Framework Core (ORM)
    ↓
Migrations (versionamento)
    ↓
PostgreSQL (persistência)
```

### Os Pilares da Nossa Modelagem

| Pilar | O que é | Exemplo |
|-------|---------|---------|
| **Entidades** | Objetos com identidade única que mudam ao longo do tempo | Game, User, Role |
| **Value Objects** | Valores imutáveis que encapsulam validação | GameTitle, Price, Email |
| **Agregates** | Grupos de entidades que trabalham juntas | Game com Title e Price |
| **Domain Events** | Eventos que representam ações importantes do negócio | GameCreatedEvent, GameUpdatedEvent |
| **Repositories** | Abstrações que fingem que não há banco de dados | IGameRepository, IUserRepository |
| **Migrations** | Histórico de mudanças no banco de dados | InitialCreate, AddUserIdentitySchema |

### 4 Tabelas Principais

O projeto tem 4 tabelas principais que modelam o domínio de negócio:

```
Games
├── Id (Guid, PK)
├── Title (string, 200 chars, UNIQUE)
├── Description (string, 2000 chars)
├── Price (decimal 18,2)
├── Genre (enum como string)
├── Status (enum como string)
└── ReleaseDate (date)
    ↓
    └── UserGameLibrary (N:1, Restrict)
            ├── Id (Guid, PK)
            ├── UserId (FK → Users, Cascade)
            ├── GameId (FK → Games, Restrict)
            ├── AcquiredAt (datetime)
            └── PricePaid (decimal 18,2)
            ↑
            └── Users (1:N)
                    ├── Id (Guid, PK)
                    ├── Name (string, 150)
                    ├── Email (string, 320, UNIQUE)
                    ├── PasswordHash (string, 500)
                    ├── RoleId (FK → Roles, Restrict)
                    └── IsActive (bool)
                    ↓
                    └── Roles (N:1)
                            ├── Id (Guid, PK)
                            ├── Name (string, 50, UNIQUE)
                            ├── Description (string, 200)
                            └── IsActive (bool)
```

### 3 Migrations que Controlam a Evolução

| # | Nome | Data | O que fez |
|---|------|------|-----------|
| 1 | **InitialCreate** | 20260325002130 | Criou tabela Games |
| 2 | **AddUserIdentitySchema** | 20260325031619 | Criou Roles, Users, UserGameLibrary |
| 3 | **FixGameCascadeDeleteToRestrict** | 20260325051235 | Mudou comportamento de deleção de Games |

### Tecnologias Utilizadas

- **Entity Framework Core** - ORM que abstrai o SQL
- **PostgreSQL** - Banco de dados robusto e escalável
- **.NET 8** - Framework moderno
- **C# 12** - Linguagem type-safe

### Benefícios da Abordagem

✅ **Type Safety** - Value Objects garantem que dados inválidos nunca são criados  
✅ **Validação Centralizada** - Regras de negócio estão no domínio, não espalhadas no código  
✅ **Testabilidade** - Lógica de negócio isolada é fácil de testar  
✅ **Manutenibilidade** - Código bem organizado é fácil de entender e modificar  
✅ **Escalabilidade** - Estrutura pronta para novos agregates e funcionalidades  
✅ **Rastreabilidade** - Migrations deixam histórico de todas as mudanças  
✅ **Integridade de Dados** - Constraints no banco garantem consistência  
✅ **Desacoplamento** - Repository Pattern permite trocar implementação sem afetar domínio  

---

## Visão Geral da Arquitetura

> **📹 O que dizer:** "Começamos aqui com uma visão geral da arquitetura. O FIAP Cloud Games é uma plataforma de distribuição de jogos, e a gente decidiu usar uma abordagem bem estruturada chamada Domain-Driven Design. Basicamente, a ideia é separar bem cada coisa: o banco de dados fica numa camada, a lógica de negócio em outra, e a API em mais uma. Isso facilita muito a manutenção, permite que a gente teste o código isoladamente, e se futuramente a gente quiser trocar de banco de dados, não vai quebrar a lógica de negócio. É tipo um quebra-cabeças bem organizado."

O projeto **FIAP Cloud Games** adota uma arquitetura em camadas baseada em **Domain-Driven Design (DDD)**, garantindo:

- ✅ Separação clara de responsabilidades
- ✅ Persistência agnóstica ao banco de dados
- ✅ Lógica de negócio centralizada no domínio
- ✅ Fácil testabilidade
- ✅ Reutilização de código

### Estrutura de Projetos

> **📹 O que dizer:** "A gente separou o projeto em 5 camadas bem definidas. No topo temos a API, que é quem fala com o mundo lá fora. Embaixo dela vem a Application, que orquestra tudo. No meio fica a Infrastructure, que cuida do banco de dados. E na base, o Domain, que é o coração do nosso negócio - é lá que fica toda a lógica de validação e regras de negócio dos jogos e usuários. Por fim, temos os testes pra garantir que tudo funciona certinho."

```
FCG.Domain          → Lógica de negócio, entidades, value objects, interfaces
FCG.Application     → Use cases, DTOs, mapeadores
FCG.Infrastructure  → Entity Framework, repositories, implementação de interfaces
FCG.API             → Endpoints, controllers, configuração da API
FCG.Tests           → Testes unitários do domínio
```

---

## Padrão de Design DDD

> **📹 O que dizer:** "DDD é um padrão que ajuda a gente a organizar código complexo focando no domínio, que é o negócio em si. A gente tem Entidades - que são tipos que mudam e têm identidade, tipo um usuário ou jogo. Depois temos Value Objects - que são valores imutáveis, tipo um email ou preço, que não mudam depois de criados. Aggregates são grupos de coisas que trabalham juntas - por exemplo, um Game é um aggregate porque encapsula tudo relacionado a um jogo. Domain Events são eventos importantes do negócio - tipo quando alguém compra um jogo. E finalmente Repositories, que são como interfaces pra guardar e recuperar esses dados do banco. Tudo isso junto deixa o código super organizado e fácil de manter."

### 1. **Entidades (Entities)**
Objetos com identidade única que persistem ao longo do tempo.

### 2. **Value Objects**
Objetos imutáveis que representam valores do domínio.

### 3. **Aggregates**
Grupos de entidades e value objects que formam uma unidade coesa.

### 4. **Domain Events**
Eventos que representam ações significativas no domínio.

### 5. **Repositories**
Abstrações para persistência de aggregates.

---

## Estrutura de Camadas

### **Camada de Domínio (FCG.Domain)**

Responsabilidades:
- Definir entidades e suas regras de negócio
- Implementar value objects
- Declarar interfaces de repositórios
- Lançar exceções de domínio
- Definir eventos de domínio

Estrutura:
```
FCG.Domain/
├── Shared/                    → Classe base Entity, interfaces comuns
│   ├── Entity.cs             → Classe base para todas as entidades
│   ├── IRepository.cs        → Interface genérica de repositório
│   └── IUnitOfWork.cs        → Interface do padrão Unit of Work
├── Games/
│   ├── Entities/
│   │   └── Game.cs           → Entidade Game com regras de negócio
│   ├── ValueObjects/
│   │   ├── GameTitle.cs      → Value object para título de jogo
│   │   └── Price.cs          → Value object para preço
│   ├── Enums/
│   │   ├── GameGenre.cs      → Gêneros de jogos disponíveis
│   │   └── GameStatus.cs     → Estados possíveis de um jogo
│   ├── Events/
│   │   ├── GameCreatedEvent.cs   → Evento disparado ao criar jogo
│   │   └── GameUpdatedEvent.cs   → Evento disparado ao atualizar jogo
│   ├── Exceptions/
│   │   └── DomainException.cs    → Exceções de domínio
│   └── Interfaces/
│       └── IGameRepository.cs    → Contrato de repositório
└── Users/
    ├── Entities/
    │   ├── User.cs           → Entidade User com validações
    │   ├── Role.cs           → Entidade Role (Admin, User)
    │   └── UserGameLibrary.cs → Entidade de relacionamento
    ├── ValueObjects/
    │   ├── Email.cs          → Value object para email
    │   └── Password.cs       → Value object para senha
    ├── Enums/
    │   └── RoleType.cs       → Tipos de roles
    ├── Exceptions/
    │   └── UserDomainException.cs
    └── Interfaces/
        ├── IUserRepository.cs
        ├── IRoleRepository.cs
        ├── IUserGameLibraryRepository.cs
        └── IPasswordHasher.cs
```

### **Camada de Aplicação (FCG.Application)**

Responsabilidades:
- Implementar use cases
- Mapear entidades para DTOs
- Orquestrar chamadas a repositórios
- Validar requisições da aplicação

### **Camada de Infraestrutura (FCG.Infrastructure)**

Responsabilidades:
- Implementar Entity Framework Core
- Configurar mappings de entidades
- Implementar padrão Unit of Work
- Implementar repositórios
- Gerenciar migrations

### **Camada de API (FCG.API)**

Responsabilidades:
- Expor endpoints REST
- Validar requisições HTTP
- Retornar respostas HTTP
- Configurar injeção de dependência

---

## Entidades do Domínio

### **1. Entidade Game**

> **📹 O que dizer:** "Vamos falar sobre a entidade Game. Essa é a estrela do nosso projeto - ela representa um jogo. Um jogo tem um título, uma descrição, um preço, um gênero, uma data de lançamento. Mas não é só isso. A gente colocou validações importantes: o título não pode ser vazio e é obrigatoriamente único - isso significa que não pode ter dois jogos com o mesmo nome. O preço não pode ser negativo - claro, né? A data de lançamento não pode ser no passado. E quando a gente cria um jogo, a gente dispara um evento dizendo 'ó, foi criado um jogo', isso é útil depois pra outras partes do sistema saber que algo importante aconteceu."

**Responsabilidade**: Representar um jogo no sistema.

**Propriedades:**
- `Id` (Guid) - Identificador único
- `Title` (GameTitle) - Título do jogo (value object)
- `Description` (string) - Descrição (máx 2000 caracteres)
- `Price` (Price) - Preço (value object)
- `Genre` (GameGenre) - Gênero do jogo (enum: Action, RPG, Strategy, Sports, Puzzle, Other)
- `Status` (GameStatus) - Estado (enum: Active, Inactive, ComingSoon)
- `ReleaseDate` (DateOnly) - Data de lançamento
- `CreatedAt` (DateTime) - Data de criação
- `UpdatedAt` (DateTime?) - Data de última atualização
- `DomainEvents` (IReadOnlyCollection) - Eventos de domínio

**Regras de Negócio:**
- ✅ Título é obrigatório e único (máx 200 caracteres)
- ✅ Descrição não pode exceder 2000 caracteres
- ✅ Preço não pode ser negativo
- ✅ Data de lançamento não pode ser anterior à data atual
- ✅ Status padrão ao criar é "Active"
- ✅ Dispara evento `GameCreatedEvent` ao ser criado
- ✅ Dispara evento `GameUpdatedEvent` ao ser atualizado
- ✅ Pode ser desativado via método `Deactivate()`

**Métodos Principais:**
```csharp
public Game(string title, string description, decimal price, GameGenre genre, DateOnly releaseDate)
public void Update(string? title, string? description, decimal? price, GameGenre? genre, DateOnly? releaseDate, GameStatus? status)
public void Deactivate()
public void ClearDomainEvents()
```

---

### **2. Entidade User**

**Responsabilidade**: Representar um usuário do sistema com autenticação e autorização.

**Propriedades:**
- `Id` (Guid) - Identificador único
- `Name` (string) - Nome do usuário (máx 150 caracteres)
- `Email` (Email) - Email (value object, máx 320 caracteres)
- `PasswordHash` (string) - Hash da senha (máx 500 caracteres)
- `RoleId` (Guid) - Referência para Role
- `IsActive` (bool) - Indica se usuário está ativo
- `CreatedAt` (DateTime) - Data de criação
- `UpdatedAt` (DateTime?) - Data de última atualização
- `Role` (Role) - Navegação para Role associado
- `GameLibrary` (IReadOnlyCollection<UserGameLibrary>) - Coleção de jogos do usuário

**Regras de Negócio:**
- ✅ Nome é obrigatório (máx 150 caracteres)
- ✅ Email é obrigatório e único
- ✅ Email deve ser validado
- ✅ Password hash é obrigatório
- ✅ RoleId não pode ser vazio
- ✅ IsActive padrão é true
- ✅ Pode ser criado apenas através do factory method `User.Create()`
- ✅ Pode atualizar nome, email e senha
- ✅ Pode mudar role
- ✅ Pode ser desativado e reativado

**Métodos Principais:**
```csharp
public static User Create(string name, string email, string passwordHash, Guid roleId)
public void UpdateName(string name)
public void UpdateEmail(string email)
public void UpdatePassword(string newHash)
public void ChangeRole(Guid roleId)
public void Deactivate()
public void Activate()
```

---

### **3. Entidade Role**

**Responsabilidade**: Representar um papel (permissão) no sistema.

**Propriedades:**
- `Id` (Guid) - Identificador único
- `Name` (string) - Nome da role (máx 50 caracteres)
- `Description` (string?) - Descrição (máx 200 caracteres)
- `IsActive` (bool) - Indica se role está ativa
- `CreatedAt` (DateTime) - Data de criação
- `UpdatedAt` (DateTime?) - Data de última atualização
- `Users` (IReadOnlyCollection<User>) - Navegação para usuários associados

**Regras de Negócio:**
- ✅ Nome é obrigatório e único (máx 50 caracteres)
- ✅ Descrição é opcional (máx 200 caracteres)
- ✅ IsActive padrão é true
- ✅ Pode ser criado apenas através do factory method `Role.Create()`
- ✅ 2 roles pré-definidas na seed: "Usuário" e "Administrador"

**Métodos Principais:**
```csharp
public static Role Create(string name, string? description = null)
internal static Role CreateSeed(Guid id, string name, string? description)
```

---

### **4. Entidade UserGameLibrary**

**Responsabilidade**: Representar o relacionamento de um usuário com seus jogos adquiridos.

**Propriedades:**
- `Id` (Guid) - Identificador único
- `UserId` (Guid) - Referência para User
- `GameId` (Guid) - Referência para Game
- `AcquiredAt` (DateTime) - Data de aquisição
- `PricePaid` (decimal) - Preço pago na aquisição
- `CreatedAt` (DateTime) - Data de criação
- `UpdatedAt` (DateTime?) - Data de última atualização
- `User` (User) - Navegação para usuário
- `Game` (Game) - Navegação para jogo

**Regras de Negócio:**
- ✅ UserId não pode ser vazio
- ✅ GameId não pode ser vazio
- ✅ PricePaid não pode ser negativo
- ✅ AcquiredAt é definido automaticamente com DateTime.UtcNow
- ✅ Índice único em (UserId, GameId) - um usuário só pode ter um jogo uma única vez
- ✅ Pode ser criado apenas através do factory method `UserGameLibrary.Create()`

**Métodos Principais:**
```csharp
public static UserGameLibrary Create(Guid userId, Guid gameId, decimal pricePaid)
```

---

## Value Objects

> **📹 O que dizer:** "Agora vamos entrar num conceito bem importante: Value Objects. Esses são objetos que representam valores, tipo dinheiro ou um email. A coisa especial deles é que são imutáveis - uma vez criados, não mudam nunca. Eles encapsulam validações, então é impossível criar um GameTitle vazio ou um Price negativo. Se alguém tentar, já lança um erro na hora. Isso é muito bom porque garante que em qualquer lugar do código, se você vê um GameTitle ou um Price, você sabe com certeza que aquilo é válido. Não precisa ficar checando em mil lugares diferentes."

### **1. GameTitle**

**Propósito**: Encapsular a lógica de validação de título de jogo.

**Características:**
- ✅ Record imutável
- ✅ Propriedade `Value` apenas leitura
- ✅ Validação no construtor:
  - Não pode ser nulo ou vazio
  - Máximo 200 caracteres
  - Texto é trimado automaticamente
- ✅ Lança `InvalidGameTitleException` em caso de validação inválida

```csharp
public record GameTitle
{
    public const int MaxLength = 200;
    public string Value { get; init; }
    
    public GameTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidGameTitleException("Title cannot be null or empty.");
        if (value.Length > MaxLength)
            throw new InvalidGameTitleException($"Title cannot exceed {MaxLength} characters.");
        
        Value = value.Trim();
    }
}
```

---

### **2. Price**

**Propósito**: Encapsular a lógica de validação de preço.

**Características:**
- ✅ Record imutável
- ✅ Propriedade `Amount` apenas leitura
- ✅ Validação no construtor:
  - Não pode ser negativo
- ✅ Lança `InvalidPriceException` em caso de valor negativo
- ✅ Armazenado como `decimal(18,2)` no banco

```csharp
public record Price
{
    public decimal Amount { get; init; }
    
    public Price(decimal amount)
    {
        if (amount < 0)
            throw new InvalidPriceException(amount);
        
        Amount = amount;
    }
}
```

---

### **3. Email**

**Propósito**: Encapsular a lógica de validação de email com regex compilado.

**Características:**
- ✅ Sealed class imutável
- ✅ Propriedade `Address` apenas leitura
- ✅ Validação com regex:
  - Padrão: `^[^@\s]+@[^@\s]+\.[^@\s]+$`
  - Compilado com timeout de 250ms
  - Case-insensitive
  - Email é convertido para lowercase
- ✅ Métodos factory:
  - `Email.Create()` - para validação completa
  - `Email.FromStorage()` - para carregar do banco (sem validação)
- ✅ Operadores de conversão implícita

```csharp
public sealed class Email
{
    private static readonly Regex _regex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ...);
    
    public string Address { get; }
    
    public static Email FromStorage(string raw) => new(raw);
    public static Email Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new UserDomainException("E-mail address cannot be null or empty.");
        if (!_regex.IsMatch(address.Trim()))
            throw new UserDomainException($"E-mail address '{address}' has an invalid format.");
        
        return new Email(address.Trim().ToLowerInvariant());
    }
    
    public static implicit operator string(Email email) => email.Address;
    public static implicit operator Email(string address) => Create(address);
}
```

---

### **4. Password**

**Propósito**: Encapsular a lógica de hash seguro de senhas.

**Detalhes**: A senha é armazenada como hash (Bcrypt) na entidade User e gerenciada através da interface `IPasswordHasher`.

---

## DbContext e Configurações

> **📹 O que dizer:** "Aqui a gente entra na camada de Infraestrutura, onde a gente cuida da persistência de dados com Entity Framework. O AppDbContext é tipo o coração dessa camada - é esse cara que se comunica com o banco de dados PostgreSQL. A gente tem várias coisas bacanas aqui: primeiro, todas as configurações das tabelas são aplicadas automaticamente, então a gente não precisa repetir código. Segundo, tem uma coisa bem útil que é a auditoria automática - toda vez que a gente atualiza algo, a data de atualização é preenchida automaticamente."

### **AppDbContext**

**Localização**: `FCG.Infrastructure/Persistence/Context/AppDbContext.cs`

**Responsabilidades:**
- ✅ Configurar modelo de dados
- ✅ Mapear entidades para tabelas
- ✅ Gerenciar relacionamentos
- ✅ Aplicar configurações de mapeamento
- ✅ Implementar lógica de auditoria automática

**DbSets Expostos:**
```csharp
public DbSet<Game> Games => Set<Game>();
public DbSet<User> Users => Set<User>();
public DbSet<Role> Roles => Set<Role>();
public DbSet<UserGameLibrary> UserGameLibrary => Set<UserGameLibrary>();
```

**Principais Características:**

1. **Constructor Injection de DbContextOptions**
```csharp
public AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) { }
```

2. **Auto-aplicação de Configurações**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    base.OnModelCreating(modelBuilder);
}
```
- Aplica automaticamente todas as configurações que implementam `IEntityTypeConfiguration<T>`

3. **Auditoria Automática**
```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    foreach (var entry in ChangeTracker.Entries<Domain.Shared.Entity>()
        .Where(e => e.State == EntityState.Modified))
    {
        entry.Property(nameof(Domain.Shared.Entity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
    }
    
    return await base.SaveChangesAsync(cancellationToken);
}
```
- Atualiza automaticamente `UpdatedAt` em todas as entidades modificadas

---

### **GameMapping (IEntityTypeConfiguration<Game>)**

**Localização**: `FCG.Infrastructure/Persistence/Mappings/GameMapping.cs`

**Configurações:**

1. **Tabela e Chave Primária**
```
Tabela: "Games"
Chave Primária: Id (Guid, não gerado automaticamente)
```

2. **Value Object GameTitle**
- Mapeado como "Owned Type"
- Coluna: "Title"
- Máximo: 200 caracteres
- Índice único

3. **Value Object Price**
- Mapeado como "Owned Type"
- Coluna: "Price"
- Tipo: decimal(18,2)

4. **Enums**
- `Genre`: Convertido para string, máx 50 caracteres
- `Status`: Convertido para string, máx 50 caracteres

5. **Propriedades de Auditoria**
- `CreatedAt`: Obrigatório, timestamp com fuso horário
- `UpdatedAt`: Opcional, timestamp com fuso horário

6. **Domain Events**
- Ignorados pelo Entity Framework (não persistidos)

---

### **UserConfiguration (IEntityTypeConfiguration<User>)**

**Localização**: `FCG.Infrastructure/Persistence/Mappings/UserConfiguration.cs`

**Configurações:**

1. **Tabela e Chave Primária**
```
Tabela: "Users"
Chave Primária: Id (Guid, não gerado automaticamente)
```

2. **Value Object Email**
- Mapeado com conversão personalizada
- Coluna: "Email"
- Máximo: 320 caracteres
- Índice único
- Conversão:
  - Para BD: `email => email.Address`
  - Do BD: `raw => Email.FromStorage(raw)`

3. **Propriedades Básicas**
- `Name`: Máx 150 caracteres, obrigatório
- `PasswordHash`: Máx 500 caracteres, obrigatório
- `IsActive`: Obrigatório
- `RoleId`: Obrigatório

4. **Relacionamentos**
- **Um-para-Muitos com Role**
  - FK: User.RoleId → Role.Id
  - Delete Behavior: Restrict (não permite deletar role com usuários)
  
- **Um-para-Muitos com UserGameLibrary**
  - FK: UserGameLibrary.UserId → User.Id
  - Delete Behavior: Cascade (deleta biblioteca ao deletar usuário)

---

### **RoleConfiguration (IEntityTypeConfiguration<Role>)**

**Localização**: `FCG.Infrastructure/Persistence/Mappings/RoleConfiguration.cs`

**Configurações:**

1. **Tabela e Chave Primária**
```
Tabela: "Roles"
Chave Primária: Id (Guid, não gerado automaticamente)
```

2. **Propriedades**
- `Name`: Máx 50 caracteres, obrigatório, índice único
- `Description`: Máx 200 caracteres, opcional
- `IsActive`: Obrigatório
- `CreatedAt`: Obrigatório
- `UpdatedAt`: Opcional

---

### **UserGameLibraryConfiguration (IEntityTypeConfiguration<UserGameLibrary>)**

**Localização**: `FCG.Infrastructure/Persistence/Mappings/UserGameLibraryConfiguration.cs`

**Configurações:**

1. **Tabela e Chave Primária**
```
Tabela: "UserGameLibrary"
Chave Primária: Id (Guid, não gerado automaticamente)
```

2. **Propriedades**
- `UserId`: Obrigatório
- `GameId`: Obrigatório
- `AcquiredAt`: Obrigatório, timestamp
- `PricePaid`: Obrigatório, decimal(18,2)
- `CreatedAt`: Obrigatório
- `UpdatedAt`: Opcional

3. **Constraints**
- Índice único em (UserId, GameId)
- Um usuário só pode ter um jogo uma única vez

4. **Relacionamentos**
- **Um-para-Muitos com User**
  - FK: UserGameLibrary.UserId → User.Id
  - Delete Behavior: Cascade
  
- **Muitos-para-Um com Game**
  - FK: UserGameLibrary.GameId → Game.Id
  - Delete Behavior: Restrict (impede deletar game com referências)

---

## Migrations

> **📹 O que dizer:** "Migrations são um conceito super importante do Entity Framework. Basicamente, são históricos de mudanças no banco de dados. A gente fez três migrations nesse projeto. A primeira criou a tabela de Games - simples, só os jogos. Depois realizamos que precisávamos de usuários pra comprar os jogos, aí criamos a segunda migration com Roles, Users e UserGameLibrary. Mas aí a gente percebeu um problema: se alguém deletasse um jogo, todos os registros de compra de um usuário era também deletados, e isso não era o que a gente queria. Então fizemos uma terceira migration só pra mudar esse comportamento. Migrations deixam tudo rastreável e reproduzível - se você clonar o projeto em outro lugar, você roda as migrations e tem a mesma estrutura de banco."

---

### 📋 RESPOSTA RÁPIDA: "O que vocês usaram de Migration?"

> **📹 O que dizer (resumido para responder na entrevista):**
>
> "A gente usou **Entity Framework Migrations** com PostgreSQL. Basicamente, migrations são arquivos de controle de versão pro banco de dados. A gente criou **3 migrations**:
>
> A primeira foi **InitialCreate** - criou a tabela Games com todos os campos: título, descrição, preço, gênero, status, data de lançamento, e campos de auditoria (CreatedAt e UpdatedAt).
>
> A segunda foi **AddUserIdentitySchema** - adicionou o sistema de identidade completo: tabela Roles com Admin e User pré-definidos, tabela Users com email único e validação, e tabela UserGameLibrary pra registrar o histórico de compras de cada usuário.
>
> A terceira foi **FixGameCascadeDeleteToRestrict** - corrigimos o comportamento de deleção. No começo, deletar um jogo deletava toda a compra na biblioteca do usuário. A gente mudou pra Restrict, que impede deletar um jogo se tiver histórico de compra, preservando a integridade histórica.
>
> Cada migration é versionada com timestamp (20260325002130, 20260325031619, etc.), o que deixa tudo rastreável. Se você clonar o projeto, é só rodar `dotnet ef database update` que todas as 3 migrations são executadas e o banco fica idêntico."

### Resumo Executivo das Migrations

```
┌─────────────────────────────────────────────┐
│ MIGRATIONS UTILIZADAS NO PROJETO           │
├─────────────────────────────────────────────┤
│ Tecnologia: Entity Framework Core           │
│ Banco de Dados: PostgreSQL                  │
│ Total de Migrations: 3                      │
│ Controle de Versão: Temporal (Timestamps)   │
│ Execução: Automática na Startup             │
└─────────────────────────────────────────────┘
```

### Tabela Comparativa das 3 Migrations

| Migration | Data | Tabelas Criadas | Mudanças Principais | Motivo |
|-----------|------|-----------------|-------------------|--------|
| **InitialCreate** | 2026-03-25 002130 | Games | Criou tabela de jogos com validações e índice único em Title | MVP inicial - só jogos |
| **AddUserIdentitySchema** | 2026-03-25 031619 | Roles, Users, UserGameLibrary | Criou sistema completo de usuários + histórico de compras + seed de 2 roles | Expandir para multi-usuário |
| **FixGameCascadeDeleteToRestrict** | 2026-03-25 051235 | Nenhuma (alter) | Alterou behavior FK de Game: de Cascade → Restrict | Preservar histórico de compras |

### Como as Migrations Funcionam

```csharp
// 1. Entity Framework detecta mudanças no modelo
public class Game : Entity
{
    public GameTitle Title { get; private set; }  // ← Mudança detectada
}

// 2. Developer cria migration
dotnet ef migrations add NomeDoMigration

// 3. EF gera arquivo de migração em: Migrations/[timestamp]_NomeDoMigration.cs

// 4. Na execução (automática na startup)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();  // ← Roda todas as migrations pendentes
}

// 5. PostgreSQL executa o SQL gerado
// CREATE TABLE, ALTER TABLE, CREATE INDEX, etc.
```

### Benefícios das Migrations Utilizadas

✅ **Versionamento** - Cada mudança é um arquivo rastreável com timestamp  
✅ **Reproduzibilidade** - Mesmo banco em qualquer máquina  
✅ **Histórico** - Consegue ver todas as mudanças que foram feitas  
✅ **Automático** - Na startup, detecta mudanças pendentes  
✅ **Reversível** - Pode fazer rollback com `dotnet ef database update <MigrationName>`  
✅ **Seguro** - Transactions garantem que ou tudo é executado ou nada é  
✅ **Organizado** - Não tem SQL solto espalhado no código  

---

### **Histórico de Migrations**

As migrations garantem versionamento e reproduzibilidade das alterações no schema.

#### **Migration 1: InitialCreate (20260325002130)**

> **📹 O que dizer:** "Na primeira migration, a gente criou a tabela Games. Bem simples: tem o ID do jogo, o título, descrição, preço, gênero, status, data de lançamento, e data de criação. O título é único - não pode ter dois jogos iguais. Tudo começou por aqui."

**Objetivo**: Criar estrutura inicial com tabela Games.

**Alterações:**

1. **Criação da Tabela Games**
```sql
CREATE TABLE "Games" (
    "Id" uuid NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Description" character varying(2000) NOT NULL,
    "Price" numeric(18,2) NOT NULL,
    "Genre" character varying(50) NOT NULL,
    "Status" character varying(50) NOT NULL,
    "ReleaseDate" date NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone
);
```

2. **Índice Único em Title**
```sql
CREATE UNIQUE INDEX "IX_Games_Title" ON "Games"("Title");
```

3. **Chave Primária**
```sql
ALTER TABLE "Games" ADD PRIMARY KEY ("Id");
```

---

#### **Migration 2: AddUserIdentitySchema (20260325031619)**

> **📹 O que dizer:** "Na segunda migration, a gente adicionou todo o sistema de usuários. Criamos a tabela de Roles - que são tipos de usuário, como 'Administrador' e 'Usuário Normal'. Depois criamos a tabela de Users com email, senha, nome, e referência pra role. E criamos também a UserGameLibrary que é tipo um carrinho de compras - registra qual usuário comprou qual jogo, quando comprou, e quanto pagou. A gente também seeded duas roles aqui pra começar: a de Administrador que pode cadastrar jogos, e a de Usuário que pode comprar. Importante: usuários não podem ficar órfãos - quando você deleta um usuário, todos os seus dados na biblioteca são deletados automaticamente."

**Objetivo**: Adicionar esquema de identidade com usuários, roles e biblioteca de jogos.

**Alterações:**

1. **Criação da Tabela Roles**
```sql
CREATE TABLE "Roles" (
    "Id" uuid NOT NULL PRIMARY KEY,
    "Name" character varying(50) NOT NULL UNIQUE,
    "Description" character varying(200),
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone
);
```

2. **Criação da Tabela Users**
```sql
CREATE TABLE "Users" (
    "Id" uuid NOT NULL PRIMARY KEY,
    "Name" character varying(150) NOT NULL,
    "Email" character varying(320) NOT NULL UNIQUE,
    "PasswordHash" character varying(500) NOT NULL,
    "RoleId" uuid NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    FOREIGN KEY ("RoleId") REFERENCES "Roles"("Id") ON DELETE RESTRICT
);
```

3. **Criação da Tabela UserGameLibrary**
```sql
CREATE TABLE "UserGameLibrary" (
    "Id" uuid NOT NULL PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "GameId" uuid NOT NULL,
    "AcquiredAt" timestamp with time zone NOT NULL,
    "PricePaid" numeric(18,2) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("GameId") REFERENCES "Games"("Id") ON DELETE CASCADE
);
```

4. **Seeding de Dados Iniciais**
- Role "Usuário" (ID: 11111111-1111-1111-1111-111111111111)
  - Descrição: "Acesso à plataforma e biblioteca de jogos."
- Role "Administrador" (ID: 22222222-2222-2222-2222-222222222222)
  - Descrição: "Pode cadastrar jogos, administrar usuários e criar promoções."

5. **Índices Criados**
- Índice único em Roles.Name
- Índice único em Users.Email
- Índice unique em (UserGameLibrary.UserId, UserGameLibrary.GameId)

---

#### **Migration 3: FixGameCascadeDeleteToRestrict (20260325051235)**

> **📹 O que dizer:** "Aqui a gente teve que corrigir algo importante. No começo, quando alguém deletava um jogo, a biblioteca de todos os usuários perdia aquele jogo também. Mas a gente percebeu que isso era ruim - se um usuário comprou um jogo em 2025, ele deveria continuar vendo esse jogo na biblioteca dele, mesmo que o jogo seja removido do catálogo em 2026. Então mudamos: agora você não pode deletar um jogo se ele tiver referências na biblioteca. Isso preserva o histórico de compras dos usuários. Essa foi uma mudança importante que exigiu uma migration separada."

**Objetivo**: Alterar comportamento de deleção entre UserGameLibrary e Games.

**Mudança:**
- **De**: `DeleteBehavior.Cascade` (deletar relacionamento ao deletar game)
- **Para**: `DeleteBehavior.Restrict` (restringe deleção de game se houver referências)

**Justificativa**: 
- Preserva integridade histórica da biblioteca do usuário
- Impede deleção acidental de jogos
- Força tratamento explícito de deleção de jogos

**Script:**
```sql
ALTER TABLE "UserGameLibrary"
DROP CONSTRAINT "FK_UserGameLibrary_Games_GameId";

ALTER TABLE "UserGameLibrary"
ADD CONSTRAINT "FK_UserGameLibrary_Games_GameId"
    FOREIGN KEY ("GameId") REFERENCES "Games"("Id") ON DELETE RESTRICT;
```

---

## Relacionamentos e Integridade Referencial

### **Diagrama de Relacionamentos**

```
┌─────────────────┐
│      Game       │
├─────────────────┤
│ Id (PK)         │
│ Title           │
│ Description     │
│ Price           │
│ Genre           │
│ Status          │
│ ReleaseDate     │
│ CreatedAt       │
│ UpdatedAt       │
└────────┬────────┘
         │
         │ 1:N (Restrict)
         │
         │
┌────────▼──────────────────┐
│  UserGameLibrary          │
├───────────────────────────┤
│ Id (PK)                   │
│ UserId (FK → User)        │
│ GameId (FK → Game)        │◄───────────────┐
│ AcquiredAt                │               │
│ PricePaid                 │               │
│ CreatedAt                 │        Constraint Unique:
│ UpdatedAt                 │        (UserId, GameId)
└────────┬────────────────┬─┘
         │                │
         │ N:1 (Cascade)  │ N:1 (Restrict)
         │                │
         │                │
┌────────▼──────┐   ┌─────▼───────┐
│     User      │   │    Game     │
├───────────────┤   │             │
│ Id (PK)       │   │ (Same as    │
│ Name          │   │  above)     │
│ Email         │   │             │
│ PasswordHash  │   └─────────────┘
│ RoleId (FK)   │
│ IsActive      │
│ CreatedAt     │
│ UpdatedAt     │
└────────┬──────┘
         │
         │ N:1 (Restrict)
         │
         │
┌────────▼──────┐
│      Role     │
├───────────────┤
│ Id (PK)       │
│ Name (Unique) │
│ Description   │
│ IsActive      │
│ CreatedAt     │
│ UpdatedAt     │
└───────────────┘
```

---

### **Detalhes de Cada Relacionamento**

#### **1. User → Role (N:1)**
- **Tipo**: Muitos para Um
- **FK**: User.RoleId
- **Delete Behavior**: RESTRICT
- **Motivo**: Um usuário deve ter exatamente uma role. Não permite deletar role com usuários ativos.
- **Navegação**: User.Role (propriedade de navegação)

#### **2. UserGameLibrary ↔ User (N:1)**
- **Tipo**: Muitos para Um
- **FK**: UserGameLibrary.UserId
- **Delete Behavior**: CASCADE
- **Motivo**: Ao deletar um usuário, toda sua biblioteca de jogos é removida (dados orfãos).
- **Navegação**: 
  - UserGameLibrary.User (propriedade de navegação)
  - User.GameLibrary (coleção de navegação)

#### **3. UserGameLibrary ↔ Game (N:1)**
- **Tipo**: Muitos para Um
- **FK**: UserGameLibrary.GameId
- **Delete Behavior**: RESTRICT
- **Motivo**: Preserva histórico de compras. Impede deletar um jogo se há referências na biblioteca.
- **Navegação**: UserGameLibrary.Game (propriedade de navegação)

#### **4. Índice Único em UserGameLibrary**
- **Colunas**: (UserId, GameId)
- **Motivo**: Um usuário só pode possuir um jogo uma única vez (evita duplicatas)

---

## Decisões Arquiteturais

> **📹 O que dizer:** "Agora a gente vai falar sobre as decisões que a gente tomou ao desenhar essa arquitetura. Essas decisões não foram por acaso - cada uma resolvia um problema ou facilitava a manutenção."

### **1. Domain-Driven Design (DDD)**

> **📹 O que dizer:** "A gente escolheu DDD porque o projeto é um negócio real - não é só um CRUD de dados. A gente tem regras de negócio complexas: um usuário pode só comprar um jogo uma vez, um preço não pode ser negativo, um email precisa ser válido. DDD deixa a gente expressar essas regras direto no código de um jeito que faz sentido pra quem entende de negócio, não só de código."

**Decisão**: Implementar DDD com separação clara entre camadas.

**Benefícios**:
- ✅ Lógica de negócio isolada no domínio
- ✅ Fácil de testar entidades
- ✅ Reutilização de código
- ✅ Persistência agnóstica

**Implementação**:
- Entidades com factory methods
- Value objects imutáveis
- Domain events
- Interfaces de repositório no domínio

---

### **2. Entidades com ID do Tipo Guid**

> **📹 O que dizer:** "A gente usou GUIDs como IDs ao invés de números sequenciais. Por quê? Primeiro, GUIDs são globalmente únicos, então se a gente tiver vários servidores criando entidades, não vai ter conflisão. Segundo, não são sequenciais, então alguém não consegue descobrir quantos jogos existem só vendo os IDs. Terceiro, a gente pode gerar o ID no cliente ou no servidor sem problema. É um trade-off: IDs numéricos são mais legaveis, mas GUIDs são mais seguros e flexíveis."

**Decisão**: Usar `Guid` como tipo de identificador ao invés de `int` incremental.

**Benefícios**:
- ✅ Identificadores globalmente únicos
- ✅ Não sequenciais (melhora segurança)
- ✅ Possibilita gerar IDs no cliente ou em múltiplos servidores
- ✅ Facilita replicação de dados

**Implementação**:
```csharp
public Guid Id { get; protected set; }

protected Entity()
{
    Id = Guid.NewGuid();
    CreatedAt = DateTime.UtcNow;
}
```

**Configuração EF**:
```csharp
builder.Property(g => g.Id).ValueGeneratedNever();
```

---

### **3. Value Objects com Validação**

> **📹 O que dizer:** "Value Objects são incrível porque a gente coloca toda a validação no construtor. Então se alguém tenta criar um GameTitle vazio, a gente lança um erro na hora. Não deixa passar. Isso é muito melhor que validar em cada lugar que usa GameTitle. É um exemplo do que chamam de \'design by contract\' - a gente coloca um contrato: \"se você quer criar um GameTitle, ele TEM que ser válido\". Se não for, falha. Não deixa estado inválido existir no sistema."

**Decisão**: Implementar value objects imutáveis com validação nos construtores.

**Benefícios**:
- ✅ Lógica de validação centralizada
- ✅ Impossível criar valor inválido
- ✅ Tipo seguro
- ✅ Semanticamente rico

**Exemplos**:
- `GameTitle`: Validação de comprimento e nulidade
- `Price`: Validação de valor negativo
- `Email`: Validação com regex

---

### **4. Factory Methods em Entidades**

> **📹 O que dizer:** "A gente usa factory methods ao invés de construtores públicos. Isso é tipo uma porta de entrada controlada. Quando você quer criar um User, você chama User.Create(), que faz todas as validações antes de realmente criar o usuário. Se algo estiver errado, a gente já rejeita ali. Se estiver tudo certo, a gente cria. Isso garante que nunca vai existir um User inválido no sistema."

**Decisão**: Usar factory methods estáticos ao invés de construtores públicos.

**Benefícios**:
- ✅ Encapsulamento de lógica complexa
- ✅ Validação obrigatória
- ✅ Nomes descritivos
- ✅ Facilita construção de agregates

**Exemplo**:
```csharp
public static User Create(string name, string email, string passwordHash, Guid roleId)
{
    // Validações...
    return new User(name, emailVo, passwordHash, roleId);
}
```

---

### **5. Domain Events**

> **📹 O que dizer:** "Domain Events são eventos que acontecem no nosso negócio. Tipo: um jogo foi criado, um usuário comprou um jogo. Esses eventos ficam armazenados temporariamente na entidade, e depois a gente pode publicar pra outras partes do sistema saber que algo aconteceu. Isso desacopla as coisas - a entidade Game não precisa saber o que fazer quando um jogo é criado, só sabe que isso aconteceu. Outro código se inscreve nesse evento e reage."

**Decisão**: Implementar eventos de domínio para ações significativas.

**Benefícios**:
- ✅ Desacoplamento entre agregates
- ✅ Auditoria de eventos
- ✅ Possibilita publicar eventos para outras partes do sistema
- ✅ Histórico de mudanças

**Implementação**:
- `GameCreatedEvent`: Disparado ao criar jogo
- `GameUpdatedEvent`: Disparado ao atualizar jogo
- Armazenados em coleção privada e acessíveis apenas para leitura

---

### **6. Auditoria Automática com CreatedAt e UpdatedAt**

> **📹 O que dizer:** "Toda entidade do sistema tem CreatedAt e UpdatedAt. CreatedAt é preenchido quando a entidade é criada, UpdatedAt é atualizado toda vez que a entidade muda. E isso é automático - a gente não precisa lembrar de preencher. Isso é super útil pra auditoria, pra saber quem criou quando, quem modificou quando. Se depois a gente descobre um problema nos dados, a gente consegue rastrear."

**Decisão**: Adicionar campos de auditoria em toda entidade base.

**Benefícios**:
- ✅ Rastreamento de quem e quando criou/modificou
- ✅ Auditoria automática sem código manual
- ✅ Histórico para análises
- ✅ Padrão em todo o sistema

**Implementação**:
- Base class `Entity` com `CreatedAt` e `UpdatedAt`
- `SaveChangesAsync()` atualiza automaticamente `UpdatedAt`

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    foreach (var entry in ChangeTracker.Entries<Domain.Shared.Entity>()
        .Where(e => e.State == EntityState.Modified))
    {
        entry.Property(nameof(Domain.Shared.Entity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
    }
    
    return await base.SaveChangesAsync(cancellationToken);
}
```

---

### **7. Tipo Decimal para Preços**

> **📹 O que dizer:** "Preços são coisas importantes de finanças. Se a gente usasse double ou float, podia ter problemas de arredondamento - tipo 0.1 + 0.2 em floating point não dá 0.3 exato. Com decimal a gente tem precisão de centavos, nenhum problema de arredondamento. É o padrão que todo sistema bancário usa. A gente usa decimal(18,2), que quer dizer: até 18 dígitos no total, sendo 2 casas decimais."

**Decisão**: Usar `decimal(18,2)` para valores monetários.

**Benefícios**:
- ✅ Precisão exata para cálculos monetários
- ✅ Evita problemas de arredondamento do `double` ou `float`
- ✅ Padrão internacional para finanças

**Configuração**:
```csharp
price.Property(p => p.Amount)
    .HasColumnType("decimal(18,2)")
    .IsRequired();
```

---

### **8. DateOnly para Datas Sem Hora**

**Decisão**: Usar `DateOnly` para data de lançamento de jogos.

**Benefícios**:
- ✅ Semântica clara (apenas data, sem hora)
- ✅ Não se confunde com datetime
- ✅ Menor consumo de armazenamento
- ✅ Tipo native do .NET 6+

**Mapeamento**:
```csharp
builder.Property(g => g.ReleaseDate)
    .HasColumnType("date")
    .IsRequired();
```

---

### **9. Enums Armazenados como Strings**

> **📹 O que dizer:** "A gente armazena enums como strings no banco ao invés de números. Então em vez de ter um 0 ou 1 pra representar o status, a gente tem 'Active' ou 'Inactive'. Por quê? Porque quando você olha direto na tabela do banco, consegue entender do que se trata. Ninguém olha pra um 0 e sabe o que é. Mas 'Active'? Óbvio. Além disso, se a gente quiser adicionar um novo status, é super fácil."

**Decisão**: Converter enums para strings no banco de dados.

**Benefícios**:
- ✅ Legibilidade direta no banco
- ✅ Facilita queries manuais
- ✅ Sem risco de números mágicos
- ✅ Fácil adicionar novos valores

**Enums Utilizados**:
- `GameGenre`: Action, RPG, Strategy, Sports, Puzzle, Other
- `GameStatus`: Active, Inactive, ComingSoon
- `RoleType`: User, Admin

**Configuração**:
```csharp
builder.Property(g => g.Genre)
    .HasConversion<string>()
    .HasMaxLength(50)
    .IsRequired();
```

---

### **10. Unit of Work Pattern**

> **📹 O que dizer:** "Unit of Work é um padrão que coordena múltiplos repositórios trabalhando juntos. Imagina que você quer comprar um jogo: você precisa caregar o user, carregar o game, criar um UserGameLibrary. Se der erro no meio, você quer que NADA seja salvo - nenhuma dessas operações fica pela metade. Unit of Work cuida disso - quando você chama SaveChanges(), ele comita tudo de uma vez, ou nada se der erro."

**Decisão**: Implementar padrão Unit of Work para coordenar múltiplos repositórios.

**Benefícios**:
- ✅ Transações coordenadas
- ✅ Controle de escopo de alterações
- ✅ Commit único de múltiplos agregates
- ✅ Facilita rollback

**Interface**:
```csharp
public interface IUnitOfWork : IDisposable
{
    IGameRepository Games { get; }
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IUserGameLibraryRepository UserGameLibrary { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

---

### **11. Repository Pattern**

> **📹 O que dizer:** "Repositories são abstrações que fingem que não tem banco de dados. De verdade, a camada de Domínio não sabe que tem um PostgreSQL lá embaixo. Pra ela, é só um repositório que guarda e busca coisas. Isso é super bom porque se a gente quiser trocar pra outro banco no futuro, a gente só muda a implementação do repositório, o domínio não precisa saber. Além disso, pra testes, a gente coloca um repositório fake em memória."

**Decisão**: Abstrair acesso a dados através de repositories.

**Benefícios**:
- ✅ Lógica de persistência isolada
- ✅ Fácil testar (mock repositories)
- ✅ Agnóstico ao banco de dados
- ✅ Facilita mudança de ORM no futuro

**Interfaces no Domínio**:
```csharp
public interface IGameRepository : IRepository<Game>
{
    // Métodos específicos de Game
}

public interface IUserRepository : IRepository<User>
{
    // Métodos específicos de User
}
```

---

### **12. Propriedades de Navegação Privadas em Coleções**

**Decisão**: Usar listas privadas com exposição apenas de `IReadOnlyCollection`.

**Benefícios**:
- ✅ Proteção contra modificação indevida
- ✅ Controle total sobre adicionar/remover items
- ✅ Invariantes de domínio preservados
- ✅ Força uso de métodos de negócio

**Exemplo**:
```csharp
private readonly List<UserGameLibrary> _gameLibrary = [];
public IReadOnlyCollection<UserGameLibrary> GameLibrary => _gameLibrary.AsReadOnly();
```

---

### **13. Comportamentos de Deleção (Delete Behaviors)**

> **📹 O que dizer:** "Delete Behavior é uma coisa importante nas relações. A gente tem dois principais: Cascade e Restrict. Cascade quer dizer que quando você deleta o pai, os filhos vão embora junto. Por exemplo, quando um usuário é deletado, toda a biblioteca dele (que são filhos) é deletada automaticamente. Restrict quer dizer o oposto: você não consegue deletar o pai se tiver filhos. Por exemplo, um jogo não pode ser deletado se ainda existem referências dele na biblioteca. A gente escolheu assim pra preservar o histórico de compras, que é importante."

**Decisão**: Usar RESTRICT e CASCADE conforme a semântica.

**Regras Aplicadas**:

| Relacionamento | Delete Behavior | Motivo |
|---|---|---|
| User → Role | **RESTRICT** | Impede deletar role com usuários |
| UserGameLibrary → User | **CASCADE** | Biblioteca deletada com usuário |
| UserGameLibrary → Game | **RESTRICT** | Preserva histórico de compras |

---

### **14. Índices Únicos para Campos Naturais**

**Decisão**: Criar índices únicos para identificadores naturais.

**Aplicações**:
- `Games.Title` - Um título é único
- `Users.Email` - Um email é único por usuário
- `Roles.Name` - Um nome de role é único
- `(UserGameLibrary.UserId, GameId)` - Cada usuário tem cada jogo uma única vez

**Benefícios**:
- ✅ Garante unicidade a nível de banco
- ✅ Melhora performance de buscas
- ✅ Previne dados duplicados
- ✅ Validação dupla (aplicação + banco)

---

### **15. Timestamps UTC**

**Decisão**: Armazenar todas as datas em UTC.

**Benefícios**:
- ✅ Consistência global
- ✅ Sem ambiguidade de fuso horário
- ✅ Facilita comparações
- ✅ Padrão internacional

**Configuração Postgres**:
```sql
"CreatedAt" timestamp with time zone NOT NULL
```

---

## Índices e Constraints

### **Tabela Games**

| Nome | Tipo | Colunas | Única | Motivo |
|---|---|---|---|---|
| PK_Games | Primary Key | Id | Sim | Identificador único |
| IX_Games_Title | Index | Title | Sim | Garantir títulos únicos |

**Constraints**:
- `Title NOT NULL`: Título obrigatório
- `Genre NOT NULL`: Gênero obrigatório
- `Status NOT NULL`: Status obrigatório
- `ReleaseDate NOT NULL`: Data de lançamento obrigatória
- `CreatedAt NOT NULL`: Data de criação obrigatória
- `Price CHECK (Price >= 0)`: Preço não negativo (validação em domínio)
- `Description MAX 2000`: Descrição com limite (validação em domínio)

---

### **Tabela Users**

| Nome | Tipo | Colunas | Única | Motivo |
|---|---|---|---|---|
| PK_Users | Primary Key | Id | Sim | Identificador único |
| IX_Users_Email | Index | Email | Sim | Garantir emails únicos |
| FK_Users_Roles_RoleId | Foreign Key | RoleId → Roles.Id | Não | Referência a Role |

**Constraints**:
- `Name NOT NULL`: Nome obrigatório
- `Email NOT NULL`: Email obrigatório
- `PasswordHash NOT NULL`: Hash obrigatório
- `RoleId NOT NULL`: Referência obrigatória
- `IsActive NOT NULL`: Status obrigatório
- `CreatedAt NOT NULL`: Data de criação obrigatória
- `FOREIGN KEY (RoleId) ... ON DELETE RESTRICT`: Não permite deletar role com usuários

---

### **Tabela Roles**

| Nome | Tipo | Colunas | Única | Motivo |
|---|---|---|---|---|
| PK_Roles | Primary Key | Id | Sim | Identificador único |
| IX_Roles_Name | Index | Name | Sim | Garantir nomes únicos |

**Constraints**:
- `Name NOT NULL`: Nome obrigatório
- `IsActive NOT NULL`: Status obrigatório
- `CreatedAt NOT NULL`: Data de criação obrigatória

---

### **Tabela UserGameLibrary**

| Nome | Tipo | Colunas | Única | Motivo |
|---|---|---|---|---|
| PK_UserGameLibrary | Primary Key | Id | Sim | Identificador único |
| IX_UGL_User_Game | Index | (UserId, GameId) | Sim | Cada usuário tem jogo 1x |
| FK_UGL_Users_UserId | Foreign Key | UserId → Users.Id | Não | Referência a User |
| FK_UGL_Games_GameId | Foreign Key | GameId → Games.Id | Não | Referência a Game |

**Constraints**:
- `UserId NOT NULL`: Usuário obrigatório
- `GameId NOT NULL`: Jogo obrigatório
- `AcquiredAt NOT NULL`: Data de aquisição obrigatória
- `PricePaid NOT NULL`: Preço pago obrigatório
- `CreatedAt NOT NULL`: Data de criação obrigatória
- `FOREIGN KEY (UserId) ... ON DELETE CASCADE`: Biblioteca deletada com usuário
- `FOREIGN KEY (GameId) ... ON DELETE RESTRICT`: Preserva histórico de compras

---

## Resumo das Entidades e Relacionamentos

### **Core Models**

```
Game (Agregado raiz)
├── Propriedades base (Id, CreatedAt, UpdatedAt)
├── Propriedades de negócio
│   ├── Title (GameTitle - VO)
│   ├── Description
│   ├── Price (Price - VO)
│   ├── Genre (Enum)
│   ├── Status (Enum)
│   └── ReleaseDate
├── Domain Events
│   ├── GameCreatedEvent
│   └── GameUpdatedEvent
└── Métodos
    ├── Update()
    ├── Deactivate()
    └── ClearDomainEvents()

Role (Entidade)
├── Propriedades base
├── Propriedades de negócio
│   ├── Name (Unique)
│   ├── Description
│   └── IsActive
├── Navegação
│   └── Users (1:N)
└── Factory
    ├── Create()
    └── CreateSeed()

User (Agregado raiz)
├── Propriedades base
├── Propriedades de negócio
│   ├── Name
│   ├── Email (Email - VO)
│   ├── PasswordHash
│   ├── RoleId (FK)
│   └── IsActive
├── Navegação
│   ├── Role (N:1)
│   └── GameLibrary (1:N)
├── Factory
│   └── Create()
└── Métodos
    ├── UpdateName()
    ├── UpdateEmail()
    ├── UpdatePassword()
    ├── ChangeRole()
    ├── Deactivate()
    └── Activate()

UserGameLibrary (Entidade de Junção)
├── Propriedades base
├── Propriedades de negócio
│   ├── UserId (FK)
│   ├── GameId (FK)
│   ├── AcquiredAt
│   └── PricePaid
├── Navegação
│   ├── User
│   └── Game
├── Constraint Único
│   └── (UserId, GameId)
└── Factory
    └── Create()
```

---

---

## Como Tudo Funciona Junto

> **📹 O que dizer:** "Agora vamos ver como é que isso tudo funciona junto. Imagina que alguém quer comprar um jogo. A requisição chega na API, vai pra camada de Application, que orquestra tudo. A Application pede pro repositório que carregue o User do banco via Infrastructure. O User vem como uma entidade do Domínio, com toda a lógica de negócio dentro dele. Igual pra o Game. Depois a Application cria um UserGameLibrary chamando User.Create(), que já faz todas as validações. Se tudo ok, a Application pede pro Unit of Work salvar. O Unit of Work manda pro DbContext, que converte as entidades em SQL, executa no PostgreSQL. Se der erro, nada é salvo. Se der tudo certo, os Domain Events são disparados e alguém fica sabendo que a compra aconteceu. Tudo isso é coordenado, transacional, e seguro. É bem diferente de só clicar no banco sem pensar."

---

## Fluxos Principais de Dados

### **Criar um Jogo**

```
API Controller
    ↓
Application Use Case
    ↓
Domain Factory (Game.Create)
    ↓ Validações
Domain Event (GameCreatedEvent)
    ↓
Repository Add
    ↓
Unit of Work Save
    ↓
Database INSERT
```

---

### **Comprar um Jogo**

```
API Controller
    ↓
Application Use Case
    ↓
UserRepository.GetById() → Load User
    ↓
GameRepository.GetById() → Load Game
    ↓
Domain Factory (UserGameLibrary.Create)
    ↓ Validações
Repository Add
    ↓
Unit of Work Save
    ↓
Database INSERT (com FK integrity check)
```

---

### **Atualizar um Jogo**

```
API Controller
    ↓
Application Use Case
    ↓
GameRepository.GetById() → Load Game
    ↓
Domain Method (Game.Update)
    ↓ Validações
Domain Event (GameUpdatedEvent)
    ↓
Unit of Work Save
    ↓
SaveChangesAsync() auto-updates UpdatedAt
    ↓
Database UPDATE
```

---

## Conclusão

> **📹 O que dizer:** "Vamos resumir o que a gente fez aqui. A gente criou uma plataforma de distribução de jogos com uma arquitetura bem pensada. Usamos Domain-Driven Design pra deixar o código focado no negócio real, não em detalhes técnicos. A gente separou bem as responsabilidades: o Domain cuida das regras de negócio, a Application orquestra, a Infrastructure cuida do banco. Temos Value Objects que garantem que dados inválidos nunca existem no sistema. Temos Domain Events pra deixar o sistema desacoplado. E temos um banco de dados bem estruturado com migrations versionadas. Tudo isso junto faz um código que é fácil de entender, fácil de manter, fácil de testar, e fácil de evoluir. Se a gente precisar adicionar novas funcionalidades amanhã, a gente consegue fazer sem quebrar nada que já existe. E é por isso que essa arquitetura é legal: ela é feita pra crescer."

A arquitetura implementada no projeto **FIAP Cloud Games** demonstra:

✅ **Fundação Sólida em DDD**: Separação clara de responsabilidades
✅ **Type Safety**: Value Objects e enums fortes
✅ **Auditoria Integrada**: Timestamps automáticos
✅ **Integridade Referencial**: Constraints bem definidos
✅ **Manutenibilidade**: Código limpo e testável
✅ **Escalabilidade**: Pronto para novos agregados

A modelagem de dados e Entity Framework estão alinhados com boas práticas da indústria, facilitando manutenção, evolução e colaboração em equipe.

---

**Versão**: 2.0  
**Data**: 27 de Abril de 2026  
**Status**: Documentação Completa com Narração para Vídeo
