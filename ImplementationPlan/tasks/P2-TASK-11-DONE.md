# TASK-11 — Asset Groups Feature (CRUD + Member Management)

## Description

Implement asset group management including member assignment.

Endpoints: `GET /api/v1/groups`, `POST /api/v1/groups`, `GET /api/v1/groups/{id}`, `PUT /api/v1/groups/{id}`, `DELETE /api/v1/groups/{id}`, `POST /api/v1/groups/{id}/assets`, `DELETE /api/v1/groups/{id}/assets/{assetId}`

`DELETE /api/v1/groups/{id}` triggers soft-delete on the group and cascade soft-delete on all Goals and DashboardCharts linked to it.

## Dependencies

TASK-03, TASK-06, TASK-07, TASK-10

## Status

Done
