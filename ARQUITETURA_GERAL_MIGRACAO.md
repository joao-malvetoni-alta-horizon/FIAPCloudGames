# 🎮 FIAP Cloud Games - Guia Completo para Apresentação em Vídeo

> **Este documento é um roteiro de apresentação.** Leia as caixas 📹 "O que dizer:" em voz alta enquanto mostra o código na tela.

---

## 1️⃣ INTRODUÇÃO - O Projeto

> **📹 O que dizer:**
> 
> "Oi! Bem-vindo ao FIAP Cloud Games. Esse é um projeto de uma plataforma para distribuição de jogos online, tipo uma Steam ou Epic Games. A gente desenvolveu esse projeto usando tecnologias modernas do .NET, com uma arquitetura bem pensada chamada Domain-Driven Design. Hoje vou te mostrar como a gente modelou os dados, como decidimos guardar as coisas no banco, quais foram as decisões técnicas importantes, e como todas as peças se encaixam. Vamos começar?"

### O que é este projeto?

- 🎮 **Plataforma de distribuição de jogos**
- 👥 **Sistema de usuários com autenticação**
- 🛒 **Biblioteca de jogos para cada usuário**
- 🔐 **Controle de permissões com roles**
- 📊 **Banco de dados PostgreSQL**

---

## 2️⃣ ARQUITETURA GERAL

> **📹 O que dizer:**
>
> "A primeira coisa que a gente fez foi pensar em como organizar o código. A gente não quis sair criando arquivos aleatoriamente. Decidimos usar uma arquitetura em camadas bem definidas, baseada num padrão chamado DDD - Domain-Driven Design. 
>
> Isso quer dizer: a gente separa bem cada responsabilidade. Na base, tem a camada de Domínio - que é onde fica toda a lógica de negócio, as regras importantes. Acima dela, tem a camada de Aplicação - que orquestra tudo, coordena as operações. Depois tem a Infraestrutura - que cuida do banco de dados, Entity Framework, repositories. E no topo, a API - que é a interface com o mundo lá fora.
>
> Por que fazer assim? Porque quando a lógica de negócio fica isolada no Domínio, fica fácil testar, fácil mover, fácil evoluir. Se futuramente a gente quiser trocar de banco de dados, a gente só mexe na Infraestrutura, o Domínio não sabe que mudou nada."

### Estrutura em 5 Camadas

```
┌─────────────────────────────────────┐
│      FCG.API (Minimal APIs)         │  ← Interface com o cliente
├─────────────────────────────────────┤
│    FCG.Application (Use Cases)      │  ← Orquestração
├─────────────────────────────────────┤
│  FCG.Infrastructure (EF + DB)       │  ← Persistência
├─────────────────────────────────────┤
│    FCG.Domain (Lógica de Negócio)   │  ← Coração do sistema
├─────────────────────────────────────┤
│      FCG.Tests (Testes)             │  ← Validação
└─────────────────────────────────────┘
```

### O que fica em cada camada?

| Camada | Responsabilidades | Exemplos de Arquivos |
|--------|-------------------|----------------------|
| **API** | Endpoints HTTP, validação de requisição, resposta HTTP | `Program.cs`, `GameEndpoints.cs` |
| **Application** | Use Cases, DTOs, Mappers, Orquestração | `CreateGameUseCase.cs`, `GameResponse.dto.cs` |
| **Infrastructure** | Entity Framework, Repositories, Unit of Work, Migrations | `AppDbContext.cs`, `GameRepository.cs`, `GameMapping.cs` |
| **Domain** | Entidades, Value Objects, Enums, Eventos, Exceções, Interfaces | `Game.cs`, `GameTitle.cs`, `GameStatus.cs`, `GameCreatedEvent.cs` |
| **Tests** | Testes unitários | `GameTests.cs` |

---

## 3️⃣ TECNOLOGIA ESCOLHIDA

> **📹 O que dizer:**
>
> "Agora vou contar por que escolhemos cada ferramenta.
>
> Escolhemos .NET 8, que é a versão mais moderna e estável do .NET. Tem performance incrível, é seguro, e tem um ecossistema gigante.
>
> Para a API, usamos Minimal APIs - que é mais simples e enxuto que controllers tradicionais. Você define os endpoints direto, sem toda aquela burocracia de classes controller.
>
> Para o banco de dados, escolhemos PostgreSQL - é gratuito, open source, robusto, confiável. Muitas empresas grandes usam em produção.
>
> Para comunicar com o banco, usamos Entity Framework Core - é um ORM que abstrai o SQL, deixa a gente trabalhar com objetos C# ao invés de escrever SQL na mão.
>
> E as migrations do EF garantem que a gente tem um histórico de todas as mudanças no banco, tudo versionado."

### Stack Tecnológico

- **.NET 8** - Framework web moderno
- **C# 12** - Linguagem moderno, type-safe
- **PostgreSQL** - Banco de dados robusto
- **Entity Framework Core** - ORM para abstração de dados
- **Minimal APIs** - API enxuta e moderna
- **Swagger/OpenAPI** - Documentação automática

---

## 4️⃣ DECISÕES PRINCIPAIS

> **📹 O que dizer:**
>
> "Vou contar as 10 decisões mais importantes que a gente tomou:"

### Decisão 1: Usar GUIDs ao invés de IDs sequenciais

> **📹 O que dizer:**
>
> "A gente usou GUIDs - que são identificadores únicos e não-sequenciais - ao invés de números como 1, 2, 3...
>
> Por que? Primeiro, GUIDs são únicos globalmente. Se a gente tiver múltiplos servidores criando dados, ninguém vai conflitar. Segundo, não são sequenciais, então alguém não consegue descobrir quantos usuários existem só contando os IDs. Terceiro, a gente pode gerar o GUID no cliente sem problema, não precisa de um banco de dados pra gerar o próximo número.
>
> Trade-off? GUIDs são maiores (16 bytes vs 4 bytes de um int), e menos legaveis (a8f3c2e1-4b9d-4e7f-9c1a-3d5e2f8a1b4c é menos fácil que '123'). Mas os benefícios superam."

**Código:**
```csharp
// Base Entity class
public abstract class Entity
{
    public Guid Id { get; protected set; }  // ← GUID, não int
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected Entity()
    {
        Id = Guid.NewGuid();  // ← Gerado automaticamente
        CreatedAt = DateTime.UtcNow;
    }
}
```

---

### Decisão 2: Value Objects com Validação no Construtor

> **📹 O que dizer:**
>
> "Value Objects são uma coisa importante em DDD. Basicamente, são objetos que representam valores - não têm identidade, são imutáveis, e encapsulam validação.
>
> Por exemplo, um GameTitle. Você não pode ter um GameTitle vazio, e não pode ter mais de 200 caracteres. Ao invés de validar isso em 50 lugares diferentes no código, a gente coloca a validação UMA VEZ no construtor. Se alguém tentar criar um GameTitle inválido, já lança erro na hora. Impossível ter um GameTitle inválido no sistema.
>
> Isso é segurança de tipo no nível de domínio. Muito mais forte que só confiar que o desenvolvedor vai validar em cada lugar."

**Código:**
```csharp
public record GameTitle
{
    public const int MaxLength = 200;
    public string Value { get; init; }  // ← Somente leitura

    public GameTitle(string value)
    {
        // ← Validação no construtor
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidGameTitleException("Title cannot be null or empty.");
        if (value.Length > MaxLength)
            throw new InvalidGameTitleException($"Title cannot exceed {MaxLength} characters.");

        Value = value.Trim();
    }
}
```

---

### Decisão 3: Factory Methods ao invés de Construtores Públicos

> **📹 O que dizer:**
>
> "Quando a gente quer criar uma entidade complexa como um User, a gente não deixa o construtor público. Ao invés disso, a gente cria um método estático chamado 'Create' que é como uma porta controlada.
>
> Quando você chama User.Create(), esse método faz TODAS as validações necessárias antes de criar o usuário. Se falta um parâmetro obrigatório, se tem algo inválido, ele já rejeita ali mesmo. Se tudo tiver ok, ele cria e retorna. Isso garante que nunca vai existir um User inválido no sistema.
>
> É tipo um 'construtor inteligente' que sabe as regras de negócio."

**Código:**
```csharp
public class User : Entity
{
    // ← Construtor privado, não público!
    private User(string name, Email email, string passwordHash, Guid roleId) : base()
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
        IsActive = true;
    }

    // ← Factory method público
    public static User Create(string name, string email, string passwordHash, Guid roleId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new UserDomainException("User name cannot be null or empty.");
        if (name.Length > 150)
            throw new UserDomainException("User name cannot exceed 150 characters.");

        var emailVo = Email.Create(email);  // ← Valida email também
        return new User(name.Trim(), emailVo, passwordHash, roleId);
    }
}
```

---

### Decisão 4: Domain Events para Desacoplamento

> **📹 O que dizer:**
>
> "Domain Events são eventos que acontecem no nosso domínio de negócio. Por exemplo: um jogo foi criado, um jogo foi atualizado, um usuário comprou um jogo. Esses eventos são super úteis porque desacoplam as coisas.
>
> Imagina: quando a gente cria um Game, a gente poderia imediatamente enviar um email pro admin dizendo 'ó, novo jogo'. Mas aí a entidade Game teria que saber sobre email, o que não faz sentido. Com Domain Events, a gente só diz 'ó, foi criado um GameCreatedEvent'. Qualquer outra parte do sistema que quiser saber disso se inscreve no evento e reage. O Game não precisa saber que existe um sistema de email."

**Código:**
```csharp
public class Game : Entity
{
    // ← List privada de events
    private readonly List<object> _domainEvents = [];
    
    // ← Propriedade pública só leitura
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    public Game(string title, string description, decimal price, GameGenre genre, DateOnly releaseDate)
    {
        // ... validações ...
        
        // ← Dispara evento quando criado
        _domainEvents.Add(new GameCreatedEvent(Id, Title.Value, CreatedAt));
    }

    public void Update(/* params */)
    {
        // ... mudanças ...
        
        // ← Dispara evento quando atualizado
        _domainEvents.Add(new GameUpdatedEvent(Id, Title.Value, UpdatedAt.Value));
    }
}
```

---

### Decisão 5: Auditoria Automática (CreatedAt e UpdatedAt)

> **📹 O que dizer:**
>
> "Toda entidade do sistema tem CreatedAt e UpdatedAt. CreatedAt é preenchido quando é criada, UpdatedAt é atualizado quando modifica. E a coisa legal é que isso é automático.
>
> Quando alguém chama SaveChangesAsync(), o DbContext olha pra todas as entidades que foram modificadas e automaticamente preenche UpdatedAt com a data/hora atual. Ninguém precisa lembrar de fazer isso.
>
> Isso é bom pra auditoria - se descobrir um problema nos dados, dá pra rastrear quando foi modificado. E também pra ordenar dados - se quiser saber os jogos mais recentes, só ordena por UpdatedAt."

**Código:**
```csharp
public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }  // ← Nullable
}

// No DbContext:
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    // ← Auditoria automática
    foreach (var entry in ChangeTracker.Entries<Entity>()
        .Where(e => e.State == EntityState.Modified))
    {
        entry.Property(nameof(Entity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
    }

    return await base.SaveChangesAsync(cancellationToken);
}
```

---

### Decisão 6: Decimal para Preços (não Double ou Float)

> **📹 O que dizer:**
>
> "Preço é algo crítico. Se a gente usasse Double ou Float, teria problemas de arredondamento. Sabe aquele problema clássico de computação: 0.1 + 0.2 = 0.30000000000000004 em floating point? Pois é, não pode acontecer com dinheiro.
>
> A gente usa Decimal, que é exato. Decimal(18,2) quer dizer: até 18 dígitos no total, sendo 2 casas decimais (centavos). É o padrão que bancos usam. Com decimal, 0.1 + 0.2 dá exatamente 0.3."

**Código:**
```csharp
public record Price
{
    public decimal Amount { get; init; }  // ← Decimal, não double

    public Price(decimal amount)
    {
        if (amount < 0)
            throw new InvalidPriceException(amount);

        Amount = amount;
    }
}

// No banco:
// Price decimal(18,2) ← 18 dígitos, 2 casas decimais
```

---

### Decisão 7: Enums como Strings no Banco

> **📹 O que dizer:**
>
> "Enums a gente salva como strings no banco, não como números.
>
> Quer dizer, a gente tem um enum GameStatus com valores Active, Inactive, ComingSoon. Ao invés de salvar como 0, 1, 2 no banco, a gente salva como 'Active', 'Inactive', 'ComingSoon'.
>
> Por que? Legibilidade! Quando você abre o banco de dados e olha um registro, consegue entender do que se trata. Ninguém olha pra um '0' e sabe que significa 'Active'. Mas 'Active'? Óbvio.
>
> Além disso, fica mais fácil adicionar novos valores sem quebrar o banco de dados."

**Código:**
```csharp
public enum GameStatus
{
    Active,
    Inactive,
    ComingSoon
}

// No mapping:
builder.Property(g => g.Status)
    .HasConversion<string>()  // ← Armazena como string
    .HasMaxLength(50)
    .IsRequired();

// No banco fica assim:
// Status character varying(50) ← 'Active', 'Inactive', etc
```

---

### Decisão 8: Índices Únicos para Dados Naturais

> **📹 O que dizer:**
>
> "A gente cria índices únicos em dados que naturalmente devem ser únicos.
>
> Por exemplo: dois games não podem ter o mesmo título. Um email não pode estar registrado duas vezes. Uma role não pode ter o mesmo nome duas vezes. Um usuário não pode ter o mesmo jogo duas vezes na biblioteca.
>
> A gente coloca esses constraints no banco de dados - o próprio banco garante que não deixa duplicar. Se alguém tentar, falha. Isso é validação em duas camadas: a aplicação não deixa criar inválido, e se por algum acaso escapa, o banco bloqueia."

**Código:**
```csharp
// GameTitle - único
builder.OwnsOne(g => g.Title, title =>
{
    title.Property(t => t.Value)
        .HasColumnName("Title")
        .HasMaxLength(GameTitle.MaxLength)
        .IsRequired();

    title.HasIndex(t => t.Value).IsUnique();  // ← Índice único
});

// Email - único
builder.HasIndex(u => u.Email).IsUnique();  // ← Índice único

// Role Name - único
builder.HasIndex(r => r.Name).IsUnique();  // ← Índice único

// (UserId, GameId) - único combinado
builder.HasIndex(ugl => new { ugl.UserId, ugl.GameId }).IsUnique();  // ← Índice único
```

---

### Decisão 9: Delete Behaviors (Restrict vs Cascade)

> **📹 O que dizer:**
>
> "Quando a gente define relacionamentos entre tabelas, a gente decide o que acontece se deletar um registro.
>
> Tem dois comportamentos principais: Cascade e Restrict.
>
> Cascade quer dizer: se deleta o pai, os filhos vão embora junto. Por exemplo: se deleta um User, toda a biblioteca dele (que são relacionamentos filhos) é deletada. Isso faz sentido porque dados órfãos são inúteis.
>
> Restrict quer dizer: você não consegue deletar o pai se tiver filhos. Por exemplo: você não consegue deletar um Game se ele ainda tem referências na biblioteca de algum usuário. Por que? Porque queremos preservar o histórico de compras - se um usuário comprou um jogo em 2025, ele deve continuar vendo na biblioteca dele mesmo que o jogo seja removido do catálogo em 2026.
>
> Essas decisões são importantes e mudam como a gente desenha o negócio."

**Código:**
```csharp
// User → Role: RESTRICT
// Você não consegue deletar uma Role se tiver usuários com essa role
builder.HasOne(u => u.Role)
    .WithMany(r => r.Users)
    .HasForeignKey(u => u.RoleId)
    .OnDelete(DeleteBehavior.Restrict);  // ← RESTRICT

// UserGameLibrary → User: CASCADE
// Quando deleta um User, toda a biblioteca dele é deletada também
builder.HasOne(u => u.User)
    .WithMany(u => u.GameLibrary)
    .HasForeignKey(gl => gl.UserId)
    .OnDelete(DeleteBehavior.Cascade);  // ← CASCADE

// UserGameLibrary → Game: RESTRICT
// Você não consegue deletar um Game se tiver referências na biblioteca
builder.HasOne(ugl => ugl.Game)
    .WithMany()
    .HasForeignKey(ugl => ugl.GameId)
    .OnDelete(DeleteBehavior.Restrict);  // ← RESTRICT
```

---

### Decisão 10: Unit of Work Pattern

> **📹 O que dizer:**
>
> "Unit of Work é um padrão que coordena múltiplos repositórios trabalhando juntos de forma transacional.
>
> Imagina que um usuário quer comprar um jogo. Você precisa: carregar o user, carregar o game, criar um UserGameLibrary, salvar tudo. Se der erro no meio - por exemplo, o game foi deletado entre carregar e tentar salvar - você quer que NADA seja salvo. Nenhuma das operações fica pela metade.
>
> Unit of Work cuida disso. Quando você chama SaveChangesAsync(), ele tenta salvar tudo de uma vez numa transação. Se algo der erro, faz rollback de tudo. Se der tudo certo, comita tudo. Tudo ou nada, nunca meio termo."

**Código:**
```csharp
public interface IUnitOfWork : IDisposable
{
    IGameRepository Games { get; }
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IUserGameLibraryRepository UserGameLibrary { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

// Na aplicação:
public class BuyGameUseCase
{
    public async Task<UserGameLibraryResponse> ExecuteAsync(BuyGameRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        var game = await _unitOfWork.Games.GetByIdAsync(request.GameId);

        var purchase = UserGameLibrary.Create(user.Id, game.Id, game.Price.Amount);

        _unitOfWork.UserGameLibrary.Add(purchase);
        
        // ← Transação: tudo junto, ou nada
        await _unitOfWork.SaveChangesAsync();

        return _mapper.MapToResponse(purchase);
    }
}
```

---

## 5️⃣ BANCO DE DADOS - ESTRUTURA

> **📹 O que dizer:**
>
> "Agora vamos ver como a gente estruturou o banco de dados. A gente tem 4 tabelas principais: Games, Users, Roles, e UserGameLibrary.
>
> Vou desenhar como elas se relacionam:"

### Diagrama de Relacionamentos

```
┌─────────────────┐
│      Games      │  ← Tabela de jogos
├─────────────────┤
│ id (PK)         │
│ title (Unique)  │
│ description     │
│ price           │
│ genre           │
│ status          │
│ release_date    │
│ created_at      │
│ updated_at      │
└────────┬────────┘
         │
         │ 1:N (Restrict)
         │
         ▼
┌─────────────────────────┐
│  UserGameLibrary        │  ← Histórico de compras
├─────────────────────────┤
│ id (PK)                 │
│ user_id (FK) ────────┐  │
│ game_id (FK) ──┐     │  │
│ acquired_at    │     │  │
│ price_paid     │     │  │
│ created_at     │     │  │
│ updated_at     │     │  │
└────┬───────────┼─────┘
     │           │
     │ N:1       │ N:1
     │ Cascade   │ Restrict
     │           │
     ▼           └──────────────────┐
  ┌──────────┐                      │
  │  Users   │  ◄────────────────────┘
  ├──────────┤
  │ id (PK)  │
  │ name     │
  │ email    │
  │ password │
  │ role_id  │ (FK) ──────┐
  │ active   │            │
  │ ...      │            │
  └──────────┘      ┌─────▼─────┐
                    │   Roles   │
                    ├───────────┤
                    │ id (PK)   │
                    │ name (U)  │
                    │ descrip.. │
                    │ active    │
                    └───────────┘
```

### Tabelas Explicadas

#### Tabela: Games
```sql
CREATE TABLE "Games" (
    "Id" uuid PRIMARY KEY,
    "Title" varchar(200) NOT NULL UNIQUE,      -- Título único
    "Description" varchar(2000),               -- Descrição
    "Price" decimal(18,2) NOT NULL,            -- Preço com 2 casas
    "Genre" varchar(50) NOT NULL,              -- Armazenado como string
    "Status" varchar(50) NOT NULL,             -- Armazenado como string
    "ReleaseDate" date NOT NULL,               -- Data de lançamento
    "CreatedAt" timestamp with time zone,      -- Data de criação (UTC)
    "UpdatedAt" timestamp with time zone       -- Data de atualização (UTC)
);
```

#### Tabela: Roles
```sql
CREATE TABLE "Roles" (
    "Id" uuid PRIMARY KEY,
    "Name" varchar(50) NOT NULL UNIQUE,        -- Nome da role, único
    "Description" varchar(200),                -- Descrição
    "IsActive" boolean NOT NULL,               -- Está ativa?
    "CreatedAt" timestamp with time zone,
    "UpdatedAt" timestamp with time zone
);
```

#### Tabela: Users
```sql
CREATE TABLE "Users" (
    "Id" uuid PRIMARY KEY,
    "Name" varchar(150) NOT NULL,              -- Nome do usuário
    "Email" varchar(320) NOT NULL UNIQUE,      -- Email único
    "PasswordHash" varchar(500) NOT NULL,      -- Hash da senha (Bcrypt)
    "RoleId" uuid NOT NULL REFERENCES "Roles"("Id")
        ON DELETE RESTRICT,                    -- Não deixa deletar role com usuários
    "IsActive" boolean NOT NULL,               -- Está ativo?
    "CreatedAt" timestamp with time zone,
    "UpdatedAt" timestamp with time zone
);
```

#### Tabela: UserGameLibrary
```sql
CREATE TABLE "UserGameLibrary" (
    "Id" uuid PRIMARY KEY,
    "UserId" uuid NOT NULL REFERENCES "Users"("Id")
        ON DELETE CASCADE,                     -- Deleta com o usuário
    "GameId" uuid NOT NULL REFERENCES "Games"("Id")
        ON DELETE RESTRICT,                    -- Não deixa deletar jogo com compras
    "AcquiredAt" timestamp with time zone NOT NULL,  -- Quando comprou
    "PricePaid" decimal(18,2) NOT NULL,       -- Preço que pagou
    "CreatedAt" timestamp with time zone,
    "UpdatedAt" timestamp with time zone,
    UNIQUE (UserId, GameId)                   -- Cada usuário tem cada jogo 1x
);
```

---

## 6️⃣ MIGRATIONS - O HISTÓRICO DO BANCO

> **📹 O que dizer:**
>
> "Agora vamos falar sobre Migrations. Migrations são um conceito super importante no Entity Framework.
>
> Basicamente, migração é um arquivo que registra uma mudança no banco de dados. A gente fez 3 migrations nesse projeto:
>
> **Migração 1**: Criou a tabela Games. Simples, só os jogos.
>
> **Migração 2**: Adicionar o sistema de usuários - criou Roles, Users, e UserGameLibrary.
>
> **Migração 3**: Corrigir o comportamento de deleção de Games. A gente percebeu que o comportamento estava errado, aí corrigiu.
>
> O legal das migrations é que elas deixam tudo rastreável. Se você clone o projeto em outro computador, roda as migrations e o banco fica exatamente igual. É tipo um git para o banco de dados."

### Migration 1: InitialCreate (20260325002130)

> **📹 O que dizer:**
>
> "Na primeira migration, a gente criou apenas a tabela Games. Bem simples. Tem o ID, o título (que é único), descrição, preço, gênero, status, data de lançamento, e as datas de criação e atualização.
>
> Nesse momento, a gente não tinha usuários ainda. Era só os jogos."

```csharp
// O que foi criado:
migrationBuilder.CreateTable(
    name: "Games",
    columns: table => new
    {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
        Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
        Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
        Genre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
        Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
        ReleaseDate = table.Column<DateOnly>(type: "date", nullable: false),
        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Games", x => x.Id);
    });

// Criou índice único no título
migrationBuilder.CreateIndex(
    name: "IX_Games_Title",
    table: "Games",
    column: "Title",
    unique: true);
```

---

### Migration 2: AddUserIdentitySchema (20260325031619)

> **📹 O que dizer:**
>
> "Na segunda migration, a gente adicionou todo o sistema de identidade.
>
> Criou a tabela Roles - com Admin e User como pré-definidas. Criou a tabela Users com email único, senha (hash), e referência para role. E criou a tabela UserGameLibrary que é tipo um carrinho de compras - registra qual usuário comprou qual jogo, quando comprou, quanto pagou.
>
> Importante: a gente já tinha cuidado para que o relacionamento Users→Roles fosse RESTRICT, então não consegue deletar uma role se tiver usuários. E UserGameLibrary→User era CASCADE, então deleta a compra quando deleta o usuário.
>
> Mas tinha um problema nessa migration: UserGameLibrary→Game era Cascade. Isso significava que se alguém deletasse um jogo, todas as compras daquele jogo sumiam da biblioteca do usuário. Isso não era certo."

```csharp
// Roles
migrationBuilder.CreateTable(
    name: "Roles",
    columns: table => new
    {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
        Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
        IsActive = table.Column<bool>(type: "boolean", nullable: false),
        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Roles", x => x.Id);
    });

// Users
migrationBuilder.CreateTable(
    name: "Users",
    columns: table => new
    {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
        Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
        PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
        RoleId = table.Column<Guid>(type: "uuid", nullable: false),
        IsActive = table.Column<bool>(type: "boolean", nullable: false),
        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Users", x => x.Id);
        table.ForeignKey(
            name: "FK_Users_Roles_RoleId",
            column: x => x.RoleId,
            principalTable: "Roles",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);  // ← RESTRICT
    });

// UserGameLibrary
migrationBuilder.CreateTable(
    name: "UserGameLibrary",
    columns: table => new
    {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        UserId = table.Column<Guid>(type: "uuid", nullable: false),
        GameId = table.Column<Guid>(type: "uuid", nullable: false),
        AcquiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        PricePaid = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_UserGameLibrary", x => x.Id);
        table.ForeignKey(
            name: "FK_UserGameLibrary_Games_GameId",
            column: x => x.GameId,
            principalTable: "Games",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);  // ← Problema! Era Cascade aqui
        table.ForeignKey(
            name: "FK_UserGameLibrary_Users_UserId",
            column: x => x.UserId,
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);  // ← Correto, Cascade
    });

// Seed de dados
migrationBuilder.InsertData(
    table: "Roles",
    columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
    values: new object[,]
    {
        { new Guid("11111111-1111-1111-1111-111111111111"), 
          new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 
          "Acesso à plataforma e biblioteca de jogos.", 
          true, 
          "Usuário", 
          null },
        { new Guid("22222222-2222-2222-2222-222222222222"), 
          new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 
          "Pode cadastrar jogos, administrar usuários e criar promoções.", 
          true, 
          "Administrador", 
          null }
    });
```

---

### Migration 3: FixGameCascadeDeleteToRestrict (20260325051235)

> **📹 O que dizer:**
>
> "Na terceira migration, a gente descobriu um problema e corrigiu.
>
> O problema era: se um jogo fosse deletado, todos os registros de compra também sumiam. Isso significa que se um usuário comprou um jogo em 2025, e em 2026 a gente remove o jogo do catálogo, esse usuário ficaria sem o registro de compra.
>
> Isso não é certo. O histórico de compras deve ser preservado para auditoria.
>
> Então a gente mudou o comportamento: agora você NÃO consegue deletar um jogo se ele tem referências na biblioteca. Se alguém quer deletar um jogo, primeiro precisa remover todas as referências. Isso é mais seguro e preserva a integridade histórica.
>
> Esse é um exemplo de evoluir a arquitetura conforme a gente aprende mais sobre o negócio."

```csharp
// O que foi mudado:
// DROP a foreign key antiga
migrationBuilder.DropForeignKey(
    name: "FK_UserGameLibrary_Games_GameId",
    table: "UserGameLibrary");

// ADD a nova, com Restrict
migrationBuilder.AddForeignKey(
    name: "FK_UserGameLibrary_Games_GameId",
    table: "UserGameLibrary",
    column: "GameId",
    principalTable: "Games",
    principalColumn: "Id",
    onDelete: ReferentialAction.Restrict);  // ← Mudou de Cascade para Restrict
```

---

## 7️⃣ COMO RODAR MIGRATIONS

> **📹 O que dizer:**
>
> "Se você clonar esse projeto, o banco de dados não vem com os dados. Você precisa executar as migrations para criar as tabelas e popular os dados iniciais.
>
> Tem dois jeitos: automático na startup da aplicação, ou manual via CLI. A gente escolheu automático - quando a aplicação inicia, ela verifica se tem migrations pendentes e roda automaticamente."

### Automático (na Startup)

```csharp
// Em Program.cs
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var retries = 10;
    for (var i = 0; i < retries - 1; i++)
        try
        {
            db.Database.Migrate();  // ← Roda migrations automaticamente
            break;
        }
        catch (Exception ex) when (i < retries - 1)
        {
            app.Logger.LogWarning(ex, "Database not ready yet, retrying in 3s...");
            Thread.Sleep(3000);
        }
}
```

### Manual (via CLI)

```bash
# Criar uma nova migration
dotnet ef migrations add NomeDoMigration

# Rodar migrations pendentes
dotnet ef database update

# Ver histórico de migrations
dotnet ef migrations list
```

---

## 8️⃣ FLUXOS DE DADOS

> **📹 O que dizer:**
>
> "Agora vamos ver como é que um fluxo de dados inteiro funciona. Vou mostrar 3 exemplos: criar um jogo, comprar um jogo, e atualizar um jogo. Isso vai deixar claro como todas as camadas trabalham juntas."

### Fluxo 1: Criar um Jogo

> **📹 O que dizer:**
>
> "Um administrador quer criar um novo jogo. Ele faz uma requisição HTTP POST para /api/games com os dados do jogo.
>
> 1. A API recebe a requisição em GameEndpoints
> 2. Passa pra camada de Application que chama CreateGameUseCase
> 3. A use case chama Game.Create() do Domínio
> 4. Game.Create() valida tudo - data de lançamento não é no passado? título tem menos de 200 caracteres? preço não é negativo?
> 5. Se passar em tudo, cria a entidade Game e dispara um evento GameCreatedEvent
> 6. A use case adiciona o Game num repositório
> 7. Chama SaveChangesAsync() do Unit of Work que comita tudo numa transação
> 8. O DbContext transforma a entidade Game em SQL INSERT
> 9. Executa no PostgreSQL
> 10. Retorna o Game criado como JSON pra API responder ao cliente
>
> Tudo isso é coordenado, seguro, e rastreável."

```
Cliente HTTP
    ↓ POST /api/games
    ↓
API (GameEndpoints)
    ↓
Application (CreateGameUseCase)
    ↓
Domain (Game.Create) ← Validações aqui!
    ↓
Domain Event (GameCreatedEvent)
    ↓
Repository.Add()
    ↓
Unit of Work.SaveChangesAsync() ← Transação
    ↓
DbContext ← Converte pra SQL
    ↓
Entity Framework ← SQL
    ↓
PostgreSQL ← INSERT
    ↓
Response JSON 200 OK + Location header
    ↓
Cliente recebe o Game criado
```

---

### Fluxo 2: Comprar um Jogo

> **📹 O que dizer:**
>
> "Agora um usuário quer comprar um jogo.
>
> 1. Requisição chega na API: POST /api/users/{userId}/games/{gameId}/buy
> 2. Use case BuyGameUseCase executa
> 3. Carrega o User do banco via Repository - isso valida que o usuário existe
> 4. Carrega o Game do banco via Repository - isso valida que o jogo existe
> 5. Chama UserGameLibrary.Create() do Domínio com os IDs
> 6. UserGameLibrary.Create() valida: UserId não é vazio? GameId não é vazio? Preço não é negativo?
> 7. Se passar, cria a entidade
> 8. Repository.Add() adiciona
> 9. Unit of Work.SaveChangesAsync() tenta salvar numa transação
> 10. DbContext converte pra SQL
> 11. PostgreSQL executa INSERT
> 12. Mas tem um detalhe: existe um índice único em (UserId, GameId)
> 13. Se esse usuário já comprou esse jogo antes, o PostgreSQL bloqueia com erro de constraint
> 14. A transação faz rollback, nada é salvo
> 15. A exceção volta pra API que retorna 400 Bad Request pro cliente
>
> Isso é super seguro - impede que alguém compre o mesmo jogo duas vezes."

```
Cliente HTTP
    ↓ POST /api/users/{userId}/games/{gameId}/buy
    ↓
API (UserGameLibraryEndpoints)
    ↓
Application (BuyGameUseCase)
    ↓
Repository.GetByIdAsync(userId)
    ↓ SELECT FROM Users WHERE Id = ?
    ↓
Database
    ↓ User encontrado
    ↓
Repository.GetByIdAsync(gameId)
    ↓ SELECT FROM Games WHERE Id = ?
    ↓
Database
    ↓ Game encontrado
    ↓
Domain (UserGameLibrary.Create) ← Validações
    ↓
Repository.Add()
    ↓
Unit of Work.SaveChangesAsync() ← Transação
    ↓
DbContext ← SQL INSERT
    ↓
PostgreSQL
    ↓
CHECK constraint (UserId, GameId) UNIQUE
    ↓
✓ Sucesso → Response 200 OK
ou
✗ Já existe → Erro → Rollback → Response 400 Bad Request
```

---

### Fluxo 3: Atualizar um Jogo

> **📹 O que dizer:**
>
> "Um admin quer atualizar os dados de um jogo.
>
> 1. Requisição: PUT /api/games/{id}
> 2. Use case UpdateGameUseCase executa
> 3. Carrega o Game do banco
> 4. Chama Game.Update() passando os novos valores
> 5. Game.Update() valida cada mudança individualmente
> 6. Se a descrição mudou, valida se não passou de 2000 caracteres
> 7. Se o preço mudou, valida se não é negativo
> 8. Faz as mudanças e dispara GameUpdatedEvent
> 9. Unit of Work.SaveChangesAsync() comita
> 10. Mas tem uma coisa legal aqui: quando SaveChangesAsync() é chamado, o DbContext automaticamente preenche UpdatedAt com a data/hora atual
> 11. DbContext converte em SQL UPDATE
> 12. PostgreSQL executa o UPDATE
> 13. Retorna o Game atualizado pro cliente
>
> Sem trabalho extra, o UpdatedAt é sempre preenchido corretamente."

```
Cliente HTTP
    ↓ PUT /api/games/{id}
    ↓
API (GameEndpoints)
    ↓
Application (UpdateGameUseCase)
    ↓
Repository.GetByIdAsync(id)
    ↓ SELECT FROM Games WHERE Id = ?
    ↓
Database
    ↓ Game encontrado
    ↓
Domain (Game.Update) ← Validações por campo
    ↓
Domain Event (GameUpdatedEvent)
    ↓
Unit of Work.SaveChangesAsync()
    ↓
DbContext ← Auto-preenche UpdatedAt
    ↓ SQL UPDATE
    ↓
PostgreSQL
    ↓
Response 200 OK com Game atualizado
```

---

## 9️⃣ RESUMO FINAL

> **📹 O que dizer:**
>
> "Deixa eu resumir o que a gente viu aqui. A gente tem uma plataforma de distribuição de jogos. A arquitetura dela é bem pensada, separada em camadas bem definidas. Cada camada tem sua responsabilidade.
>
> A camada de Domínio tem toda a lógica de negócio - é ali que a gente garante que dados inválidos nunca existem. A camada de Aplicação orquestra tudo, coordena as operações. A camada de Infraestrutura cuida do banco de dados com Entity Framework. A camada de API expõe endpoints HTTP.
>
> Utilizamos várias padrões e decisões importantes: GUIDs para identificadores, Value Objects com validação, Factory Methods, Domain Events, Auditoria Automática, Decimal para preços, Enums como strings, Índices únicos, Delete Behaviors bem pensados, e Unit of Work para transações.
>
> Tudo isso junto cria um sistema que é: seguro (dados inválidos não conseguem entrar), testável (lógica isolada é fácil de testar), mantível (código bem organizado é fácil de entender e modificar), e escalável (pronto pra crescer com novos agregados e funcionalidades).
>
> Se você quer aprender arquitetura de software moderna, esse projeto é um bom exemplo. Tá tudo aqui: padrões, melhores práticas, estrutura pensada."

### Tecnologias Utilizadas

- ✅ **.NET 8** - Framework
- ✅ **C# 12** - Linguagem
- ✅ **PostgreSQL** - Banco de dados
- ✅ **Entity Framework Core** - ORM
- ✅ **Minimal APIs** - API
- ✅ **Swagger** - Documentação

### Padrões Utilizados

- ✅ **Domain-Driven Design** - Organização
- ✅ **Repository Pattern** - Abstração de dados
- ✅ **Unit of Work** - Transações
- ✅ **Value Objects** - Segurança de tipo
- ✅ **Domain Events** - Desacoplamento
- ✅ **Factory Methods** - Construção segura
- ✅ **Minimal APIs** - API moderna

### Decisões Importantes

- ✅ GUIDs como IDs (não sequenciais)
- ✅ Validação no Domínio (não na API)
- ✅ Decimal para preços (não double)
- ✅ Enums como strings (legibilidade)
- ✅ Índices únicos (integridade)
- ✅ Delete Behaviors (Restrict vs Cascade)
- ✅ Auditoria automática (CreatedAt/UpdatedAt)

---

**Versão**: 3.0  
**Data**: 27 de Abril de 2026  
**Status**: Guia Completo para Apresentação em Vídeo  
**Tempo de apresentação estimado**: 15-20 minutos
