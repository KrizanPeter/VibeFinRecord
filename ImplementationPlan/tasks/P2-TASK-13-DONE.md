# TASK-13 — Snapshots Feature (Submit + List + Get)

## Description

Implement snapshot submission and retrieval.

`POST /api/v1/snapshots` — accepts snapshot date + array of `{ assetId, value }` for all active assets. Validates all active assets are covered. Creates `Snapshot` + `AssetSnapshot` records.

`GET /api/v1/snapshots` — paginated list of past snapshots with total net worth per snapshot.

`GET /api/v1/snapshots/{id}` — single snapshot with all asset values.

## Dependencies

TASK-12

## Status

Done
