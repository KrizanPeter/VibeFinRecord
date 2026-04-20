# TASK-01 — .NET Solution Scaffolding

## Description

Create the .NET solution with four projects wired together following Clean Architecture dependency rules. Set up all NuGet packages needed across the solution.

Projects:
- `FinClaude.Domain` — no dependencies
- `FinClaude.Application` — references Domain
- `FinClaude.Infrastructure` — references Application + Domain
- `FinClaude.Api` — references Application + Infrastructure

Key packages:
- `ErrorOr` (Application, Domain)
- `Microsoft.EntityFrameworkCore.Sqlite` (Infrastructure)
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (Infrastructure)
- `Microsoft.AspNetCore.Authentication.JwtBearer` (Api)
- `Serilog.AspNetCore` (Api)
- `xunit`, `Moq`, `FluentAssertions` (FinClaude.Unit.Tests — also scaffold this project here)

Also scaffold `FinClaude.Unit.Tests` xUnit project with these packages as part of this task.

## Dependencies

None

## Status

Done
