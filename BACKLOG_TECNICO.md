# Backlog Técnico - FCG (FIAP Cloud Games)

## 📋 Lista de Pendências Técnicas

### ✅ Implementado
- [x] **Enum para códigos de resposta padronizados** - Criado `ApiResponseCode` e `ApiResponse<T>` para respostas consistentes
- [x] **Autenticação JWT** - Implementado sistema completo de login/registro com tokens JWT
- [x] **Endpoint de compra de jogos** - Criado endpoint que usa JWT para identificar usuário (não passa mais usuário como parâmetro)

### 🔄 Em Andamento
- [ ] **Atualizar endpoints existentes** - Migrar endpoints de jogos para usar `ApiResponse<T>` padronizado
- [ ] **Validação de entrada** - Implementar validação de DTOs usando FluentValidation ou DataAnnotations
- [ ] **Logs estruturados** - Implementar Serilog para logs mais ricos
- [ ] **Testes unitários** - Criar cobertura de testes para novos use cases e serviços
- [ ] **Documentação OpenAPI** - Melhorar descrições e exemplos nos endpoints

### 📈 Melhorias de Arquitetura

#### Domain Layer
- [ ] **Value Objects adicionais** - Criar VO para Email, Password, Money
- [ ] **Domain Events** - Implementar eventos de domínio para compras, registros, etc.
- [ ] **Specifications** - Padrão Specification para consultas complexas
- [ ] **Domain Services** - Serviços de domínio para regras de negócio complexas

#### Application Layer
- [ ] **CQRS** - Separar Commands de Queries (usar MediatR)
- [ ] **FluentValidation** - Validação de entrada robusta
- [ ] **AutoMapper** - Mapeamento automático entre entidades e DTOs
- [ ] **Caching** - Implementar cache para dados frequentemente acessados

#### Infrastructure Layer
- [ ] **Repository Pattern completo** - Implementar repositórios genéricos
- [ ] **Unit of Work** - Melhorar implementação com transações distribuídas
- [ ] **Migrations** - Estratégia de versionamento de banco mais robusta
- [ ] **Health Checks** - Implementar verificações de saúde da aplicação

#### API Layer
- [ ] **Rate Limiting** - Limitar requisições por usuário/IP
- [ ] **CORS** - Configurar Cross-Origin Resource Sharing
- [ ] **Versionamento** - Implementar versionamento de API
- [ ] **Middleware de performance** - Monitoring de performance e métricas

### 🐛 Correções de Bugs e Issues

#### Segurança
- [ ] **Password Policy** - Implementar política de senhas mais robusta
- [ ] **JWT Refresh Tokens** - Implementar refresh tokens para melhor segurança
- [ ] **Input Sanitization** - Sanitizar todas as entradas do usuário
- [ ] **SQL Injection Prevention** - Verificar e corrigir vulnerabilidades (Entity Framework já protege, mas revisar)

#### Performance
- [ ] **Database Indexes** - Otimizar índices do banco de dados
- [ ] **Query Optimization** - Revisar queries N+1 e implementar eager loading
- [ ] **Pagination** - Implementar paginação em todos os endpoints de listagem
- [ ] **Async/Await** - Garantir que todos os métodos I/O sejam assíncronos

#### Usabilidade
- [ ] **Error Messages** - Melhorar mensagens de erro para o usuário final
- [ ] **API Documentation** - Documentação completa da API com exemplos
- [ ] **Request/Response Examples** - Exemplos práticos no Swagger
- [ ] **Localization** - Suporte a múltiplos idiomas

### 🧪 Qualidade de Código

#### Testes
- [ ] **Unit Tests** - Cobertura mínima de 80% do código
- [ ] **Integration Tests** - Testes de integração com banco de dados
- [ ] **E2E Tests** - Testes end-to-end com Playwright ou similar
- [ ] **Test Doubles** - Mocks, stubs e fakes para testes

#### Code Quality
- [ ] **Code Analysis** - Configurar SonarQube ou similar
- [ ] **Code Coverage** - Relatórios de cobertura de testes
- [ ] **Style Guide** - Implementar EditorConfig e regras de estilo
- [ ] **Code Reviews** - Processo de revisão de código obrigatório

### 🚀 DevOps e Deployment

#### CI/CD
- [ ] **GitHub Actions** - Pipeline de CI/CD completo
- [ ] **Docker** - Otimizar Dockerfile para produção
- [ ] **Container Orchestration** - Kubernetes ou Docker Compose para produção
- [ ] **Environment Management** - Gestão de ambientes (dev, staging, prod)

#### Monitoring
- [ ] **Application Insights** - Telemetria e monitoring
- [ ] **Logging** - Centralização de logs (ELK stack ou similar)
- [ ] **Alerting** - Alertas para erros críticos
- [ ] **Metrics** - Métricas de negócio e performance

### 📚 Documentação

#### Técnica
- [ ] **Architecture Decision Records** - Documentar decisões arquiteturais
- [ ] **API Documentation** - Documentação completa da API
- [ ] **Database Schema** - Documentação do schema do banco
- [ ] **Deployment Guide** - Guia de deployment e configuração

#### Negócio
- [ ] **Business Rules** - Documentar regras de negócio
- [ ] **User Stories** - Histórias de usuário completas
- [ ] **Use Cases** - Casos de uso detalhados
- [ ] **Domain Model** - Modelo de domínio atualizado

## 🎯 Priorização

### Alta Prioridade (Sprint 1-2)
1. Atualizar endpoints existentes para usar `ApiResponse<T>`
2. Implementar validação de entrada
3. Criar testes unitários básicos
4. Melhorar documentação OpenAPI

### Média Prioridade (Sprint 3-4)
1. Implementar CQRS com MediatR
2. Adicionar caching
3. Melhorar performance das queries
4. Implementar health checks

### Baixa Prioridade (Sprint 5+)
1. Implementar domain events
2. Adicionar rate limiting
3. Configurar CI/CD completo
4. Implementar monitoring avançado

## 📊 Métricas de Sucesso

- **Cobertura de Testes**: > 80%
- **Performance**: < 500ms para endpoints principais
- **Uptime**: > 99.5%
- **Security**: Zero vulnerabilidades críticas
- **Code Quality**: Grade A no SonarQube