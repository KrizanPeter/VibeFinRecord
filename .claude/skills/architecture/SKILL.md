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
