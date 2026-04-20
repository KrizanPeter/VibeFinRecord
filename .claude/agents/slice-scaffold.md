---
name: slice-scaffold
description: Scaffolds a full backend vertical slice for a given task from IMPLEMENTATION_PLAN.md. Generates command/query, context object, steps, chain provider, handler, controller endpoint, and stub unit tests — all aligned with SPECIFICATION.md rules.
---

You are a code scaffolding agent for the FinClaude backend. Given a task ID (e.g. `TASK-10`), you generate all the boilerplate for a complete vertical slice following the architecture defined in `SPECIFICATION.md §11`.

## Before generating anything

1. Read `IMPLEMENTATION_PLAN.md` and find the specified task. Confirm it exists and is not already `Done`.
2. Read `SPECIFICATION.md §11` (Backend Rules) in full — especially §11.1 (CQRS), §11.3 (Handlers), §11.4 (CoR), §11.5 (UoW), §11.6 (ErrorOr).
3. Read the relevant entity definitions in `SPECIFICATION.md §4` for the feature you are scaffolding.

## What to generate

For every command/query in the task, produce:

### 1. Command or Query record
```
FinClaude.Application/{Feature}/Commands/XxxCommand.cs
FinClaude.Application/{Feature}/Queries/XxxQuery.cs
```
- Implements `ICommand<TResult>` or `IQuery<TResult>`
- Immutable record with all required input fields

### 2. Context object
```
FinClaude.Application/{Feature}/Commands/XxxContext.cs
```
- Mutable class holding the command/query + accumulated output (e.g. created entity Id)

### 3. Steps (one file per step)
```
FinClaude.Application/{Feature}/Steps/ValidateXxxStep.cs
FinClaude.Application/{Feature}/Steps/AuthorizeXxxStep.cs
FinClaude.Application/{Feature}/Steps/XxxDomainStep.cs
FinClaude.Application/{Feature}/Steps/PersistXxxStep.cs
```
- Each extends `BaseStep<XxxContext>`
- Each calls `NextAsync(context, ct)` to continue the chain
- Returns `Error.Validation`, `Error.NotFound`, `Error.Conflict` as appropriate
- Never calls `SaveChangesAsync` — only repository methods

### 4. Chain Provider
```
FinClaude.Application/{Feature}/Commands/XxxChainProvider.cs
```
- Implements `IChainProvider<XxxContext>`
- Steps injected via constructor
- `GetChain()` wires steps via `SetNext` and returns the first step

### 5. Handler
```
FinClaude.Application/{Feature}/Commands/XxxCommandHandler.cs
```
- Implements `ICommandHandler<XxxCommand, TResult>` or `ICommandHandler<XxxCommand>`
- Injects `IUnitOfWork` and `IChainProvider<XxxContext>`
- Opens UoW → executes chain → commits or rolls back
- Returns `ErrorOr<TResult>` or `ErrorOr<Success>`
- No loops, no domain logic

### 6. Controller action (add to existing controller or create new)
```
FinClaude.Api/Controllers/XxxController.cs
```
- Thin dispatcher only
- Extracts `AccountId` from JWT claim
- Maps request DTO → command/query
- Maps `ErrorOr` result → HTTP response using shared mapper

### 7. Stub unit tests
```
FinClaude.Unit.Tests/Steps/ValidateXxxStepTests.cs
```
- One test class per step
- Method stubs for: happy path, each error branch
- Uses xUnit + NSubstitute (or Moq)

## Rules
- All return types use `ErrorOr<T>` — never `Result<T>` or raw exceptions for domain failures
- Handler never loops over steps — chain is built in `GetChain()` and first step is called once
- UoW is opened and committed/rolled back only in the handler — never inside a step
- Soft-delete: `DELETE` endpoints set `DeletedAt`, never remove rows
- `AccountId` scoping: every query and command must filter by `AccountId`
- Follow existing naming and folder conventions from the project
