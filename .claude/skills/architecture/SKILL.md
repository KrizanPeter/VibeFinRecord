---
name: architecture
description: Core architectural rules for the FinClaude backend. Ensures consistent Clean Architecture, CQRS, Step Pipeline, and Unit of Work behavior across all generated code.
---
## Clean Architecture Layers
- **Domain**: Entities, value objects, domain rules. No dependencies.
- **Application**: Commands, queries, DTOs, interfaces. Depends only on Domain.
- **Infrastructure**: EF Core, Identity, external services. Implements Application interfaces.
- **API**: Controllers, middleware, DI. Depends on Application + Infrastructure.

## CQRS Rules
- Commands represent write operations.
- Queries represent read operations.
- Handlers implement one of:
  - `ICommandHandler<TCommand>`
  - `ICommandHandler<TCommand, TResult>`
  - `IQueryHandler<TQuery, TResult>`

## Handler Rules
- Handlers contain **no business logic**.
- Build a context object.
- Resolve chain via `IChainProvider<TContext>`.
- Execute chain inside Unit of Work.
- On success → commit.
- On error or exception → rollback.

## Step Pipeline Rules
- Each step performs exactly **one** responsibility.
- Steps inherit from `BaseStep<TContext>`.
- Steps return `ErrorOr<Success>`.
- Steps call `NextAsync(context)` to continue.

## Unit of Work Rules
- `BeginAsync` starts a transaction.
- `CommitAsync` saves changes and commits.
- `RollbackAsync` rolls back.
- Repositories **never** call `SaveChangesAsync` directly.

## Controller Rules
- Controllers are **thin dispatchers**.
- No business logic.
- Map HTTP → command/query → handler → HTTP.
- Extract `AccountId` from JWT claim.

---

## Project placement

| Artifact | Project |
|---|---|
| Command / Query record | `FinClaude.Application/Features/{Feature}/Commands/{Op}/` |
| Context object | `FinClaude.Application/Features/{Feature}/Commands/{Op}/` |
| Handler | `FinClaude.Application/Features/{Feature}/Commands/{Op}/` |
| Query + Query Handler | `FinClaude.Application/Features/{Feature}/Queries/{Op}/` |
| DTOs | `FinClaude.Application/Features/{Feature}/DTOs/` |
| Steps | `FinClaude.Infrastructure/Features/{Feature}/Commands/Steps/` |
| ChainProvider | `FinClaude.Infrastructure/Features/{Feature}/Commands/` |
| Controller | `FinClaude.Api/Controllers/` |

---

## Type signatures

| Artifact | Base type / interface |
|---|---|
| Command (no result) | `record XyzCommand(...) : ICommand` |
| Command (with result) | `record XyzCommand(...) : ICommand<TResult>` |
| Query | `record XyzQuery(...) : IQuery<TResult>` |
| Command handler | `class XyzCommandHandler : ICommandHandler<XyzCommand, TResult>` |
| Query handler | `class XyzQueryHandler : IQueryHandler<XyzQuery, TResult>` |
| Step | `class XyzStep : BaseStep<XyzContext>` — returns `ErrorOr<Success>`, calls `NextAsync` |
| ChainProvider | `class XyzChainProvider : IChainProvider<XyzContext>` — wires steps via `SetNext` |

Handler body pattern: build context → `GetChain()` → `BeginAsync` → `ExecuteAsync` → commit or rollback → map result.

Controller pattern: `[ApiController]`, `[Authorize]`, extract `account_id` claim via `User.FindFirstValue`, dispatch to handler, return `result.ToActionResult(...)`.
