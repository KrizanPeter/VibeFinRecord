# TASK-27 — Backend Unit Tests (Steps + Handlers)

## Description

Implement unit tests in `FinClaude.Unit.Tests` targeting steps and handlers.

Per step: happy path + every possible error branch (each `Error.*` return).
Per handler: verify UoW opens + commits on success; verify UoW rolls back when chain returns error; verify UoW rolls back on exception.

Use xUnit + Moq (or NSubstitute) for mocking injected dependencies.

## Dependencies

TASK-08 through TASK-14

## Status

Waiting
