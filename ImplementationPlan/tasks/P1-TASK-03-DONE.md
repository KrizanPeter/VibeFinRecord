# TASK-03 — CQRS + Chain of Responsibility Infrastructure

## Description

Implement all architectural building blocks in `FinClaude.Application` and `FinClaude.Domain`.

Interfaces to create:
- `ICommand`, `ICommand<TResult>`, `IQuery<TResult>`
- `ICommandHandler<TCommand>` → `Task<ErrorOr<Success>>`
- `ICommandHandler<TCommand, TResult>` → `Task<ErrorOr<TResult>>`
- `IQueryHandler<TQuery, TResult>` → `Task<ErrorOr<TResult>>`
- `IStep<TContext>` → `Task<ErrorOr<Success>>`
- `IChainProvider<TContext>` → `IStep<TContext> GetChain()`
- `IUnitOfWork` → `BeginAsync`, `CommitAsync`, `RollbackAsync`

Base classes:
- `BaseStep<TContext>` — holds `_next`, exposes `SetNext`, `NextAsync`

## Dependencies

TASK-01

## Status

Done
