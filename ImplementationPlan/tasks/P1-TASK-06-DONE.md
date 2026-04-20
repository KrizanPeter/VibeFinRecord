# TASK-06 — Unit of Work Implementation

## Description

Implement `IUnitOfWork` in `FinClaude.Infrastructure` wrapping `AppDbContext`.

- `BeginAsync` — begins an EF Core transaction
- `CommitAsync` — calls `SaveChangesAsync` then commits transaction
- `RollbackAsync` — rolls back transaction
- Repositories must call only `Add`, `Update`, `Remove` on the context — never `SaveChangesAsync`
- Register `IUnitOfWork` in DI

## Dependencies

TASK-04

## Status

Done
