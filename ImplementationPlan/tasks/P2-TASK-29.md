# TASK-29 — DashboardChart Feature (CRUD + Data)

## Description

Implement the full DashboardChart feature in `FinClaude.Api`.

Endpoints:
- `GET /api/v1/dashboard/charts` — list all active charts for the account, ordered by `CreatedAt` ascending
- `POST /api/v1/dashboard/charts` — create a chart; `Pie` + `SourceType = Asset` → 400; exactly one of `AssetId`/`GroupId` required → 400
- `GET /api/v1/dashboard/charts/{id}` — get chart configuration
- `PUT /api/v1/dashboard/charts/{id}` — update name or chart type (Pie constraint enforced)
- `DELETE /api/v1/dashboard/charts/{id}` — soft-delete
- `GET /api/v1/dashboard/charts/{id}/data` — computed data points; `?range=3m|6m|1y|all` (default `all`)

Data computation rules (at query time; no pre-computed cache in MVP):

| ChartType | SourceType | Data |
|-----------|------------|------|
| Line / Bar | Asset | Time series of `AssetSnapshot.Value` for the asset, ascending by `SnapshotDate` |
| Line / Bar | AssetGroup | Sum of `AssetSnapshot.Value` for all Group assets per `SnapshotDate`, ascending |
| Pie | AssetGroup | Each Asset's value as share of Group total at the **latest** `SnapshotDate` |

- Soft-deleted Assets excluded from Group aggregation going forward; historical values preserved.
- No snapshot data → `200 OK` with `{ dataPoints: [] }`.

Steps for POST/PUT: Validate → Authorize → Domain logic → Persist

## Dependencies

TASK-03, TASK-06, TASK-07, TASK-10, TASK-11, TASK-13

## Status

Waiting
