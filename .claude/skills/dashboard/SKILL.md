---
name: dashboard
description: Rules governing DashboardChart creation, validation, update, deletion, and data computation for the FinClaude backend. Ensures consistent chart behavior and correct aggregation logic across all chart types.
---
## Chart Creation Rules
- A chart must specify `SourceType = Asset` or `SourceType = AssetGroup`.
- Exactly **one** of the following must be provided:
  - `AssetId`
  - `GroupId`
- `ChartType = Pie` is allowed **only** when `SourceType = AssetGroup`.
- Violations return **400 Bad Request**.

## Chart Update Rules
- Updating a chart must re‑validate all creation rules.
- Changing to `ChartType = Pie` requires `SourceType = AssetGroup`.
- Changing the linked Asset/Group must preserve the “exactly one” rule.

## Chart Deletion Rules
- Charts are **soft‑deleted** (set `DeletedAt`).
- Soft‑deleted charts do not appear in lists.
- Deleting a chart does **not** affect snapshots or historical data.

## Chart Data Computation Rules
- Data is computed **on demand** (no caching in MVP).
- Behavior by chart type:

### Line / Bar + Asset
- Return a time series of `AssetSnapshot.Value` for the asset.
- Ordered by `SnapshotDate` ascending.

### Line / Bar + AssetGroup
- For each snapshot date:
  - Sum `AssetSnapshot.Value` for all assets in the group.
- Ordered by `SnapshotDate` ascending.

### Pie + AssetGroup
- Use the **latest snapshot date** only.
- For each asset in the group:
  - Compute its share of the group total.
- If no snapshot data exists → return `{ dataPoints: [] }`.

## Soft‑Delete Interaction
- Soft‑deleted assets are excluded from **future** group aggregations.
- Historical values remain intact.

## Range Filtering
- Supported ranges:
  - `3m`
  - `6m`
  - `1y`
  - `all` (default)
- Range filters apply to Line/Bar charts only.
