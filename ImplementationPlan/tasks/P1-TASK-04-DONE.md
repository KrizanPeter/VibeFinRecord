# TASK-04 — EF Core + SQLite Setup

## Description

Set up `AppDbContext` in `FinClaude.Infrastructure` with EF Core + SQLite.

- Entity type configurations for all entities (separate `IEntityTypeConfiguration<T>` classes)
- `decimal(18,4)` precision on all financial columns
- Global soft-delete query filter (`HasQueryFilter(e => e.DeletedAt == null)`) on: `Asset`, `AssetGroup`, `Snapshot`, `Goal`
- `AssetSnapshot` excluded from soft-delete filter (immutable historical record)
- `SaveChanges` / `SaveChangesAsync` override to auto-set `CreatedAt`, `UpdatedAt`
- Initial EF Core migration

## Dependencies

TASK-02

## Status

Done
