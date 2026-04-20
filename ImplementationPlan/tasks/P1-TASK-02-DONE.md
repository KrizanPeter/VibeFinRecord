# TASK-02 — Domain Entities

## Description

Implement `BaseEntity` and all six domain entities in `FinClaude.Domain`.

Entities: `Account`, `Asset`, `AssetGroup`, `AssetGroupMembership`, `Snapshot`, `AssetSnapshot`, `Goal`, `DashboardChart`

Each entity extending `BaseEntity` gets: `Id`, `CreatedAt`, `UpdatedAt`, `DeletedAt`, `IsDeleted`.

Financial decimal fields (`AssetSnapshot.Value`, `Goal.TargetValue`) must be `decimal` typed — precision `(18,4)` is enforced at the EF configuration level (TASK-04).

## Dependencies

TASK-01

## Status

Done
