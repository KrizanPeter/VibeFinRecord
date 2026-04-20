---
name: domain
description: Domain-level rules for all FinClaude backend logic. Ensures consistent behavior for soft-delete, ownership, entity relationships, and domain invariants across the entire system.
---
## Soft-delete Rules
- `Asset`, `AssetGroup`, `Snapshot`, `Goal`, and `DashboardChart` use soft-delete (`DeletedAt`).
- `AssetSnapshot` is immutable and **never** soft-deleted.
- Soft-deleted entities are excluded from all future queries via global EF Core filters.
- Cascade soft-delete:
  - Deleting an **Asset** → soft-delete all linked **Goals** and **DashboardCharts**.
  - Deleting an **AssetGroup** → soft-delete all linked **Goals** and **DashboardCharts**.

## Ownership Rules
- Every entity belongs to exactly **one Account**.
- All queries and commands must be scoped by `AccountId` extracted from the JWT.
- Cross-account access must always return `404 Not Found`.

## Asset Rules
- Asset requires `Name`.
- `Institution` is optional.
- Soft-deleted assets remain visible in historical snapshots (data integrity preserved).

## AssetGroup Rules
- Many-to-many relationship via `AssetGroupMembership`.
- An Asset may belong to **zero or more** groups.
- A Group may contain **zero or more** assets.

## Goal Rules
- A Goal must link to **exactly one** of:
  - `AssetId`
  - `GroupId`
- Linking to both or neither is invalid → return `400 Bad Request`.
- Progress calculation:
  - `progress = latestValue / targetValue * 100`
- If the linked Asset or Group is soft-deleted → cascade soft-delete the Goal.

## DashboardChart Rules
- Must link to exactly one of: `AssetId` or `GroupId`.
- `ChartType = Pie` is allowed **only** when `SourceType = AssetGroup`.
- Soft-delete cascades from linked Asset or Group.
