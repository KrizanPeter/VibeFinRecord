# TASK-31 — Chart Detail Screen

## Description

Implement the full Chart Detail screen reached by tapping a chart card on the Dashboard.

- Full-size chart rendered from `GET /api/v1/dashboard/charts/{id}/data`
- Range selector tabs: 3M / 6M / 1Y / All — re-fetches with `?range=` via TanStack Query
- Edit action: update chart name or type (client-side Pie constraint enforced before submit)
- Delete action: soft-delete with confirmation dialog; navigates back to Dashboard on success

## Dependencies

TASK-30

## Status

Waiting
