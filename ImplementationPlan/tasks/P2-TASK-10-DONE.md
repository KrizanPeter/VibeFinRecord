# TASK-10 — Assets Feature (CRUD)

## Description

Implement full asset management.

Endpoints: `GET /api/v1/assets`, `POST /api/v1/assets`, `GET /api/v1/assets/{id}`, `PUT /api/v1/assets/{id}`, `DELETE /api/v1/assets/{id}`

`DELETE` triggers soft-delete on the Asset and cascade soft-delete on all Goals and DashboardCharts linked to it.

Steps per command: Validate → Authorize (account owns asset) → Domain logic → Persist

## Dependencies

TASK-03, TASK-06, TASK-07

## Status

Done
