# TASK-14 — Goals Feature (CRUD)

## Description

Implement goal management with progress calculation.

Endpoints: `GET /api/v1/goals`, `POST /api/v1/goals`, `GET /api/v1/goals/{id}`, `PUT /api/v1/goals/{id}`, `DELETE /api/v1/goals/{id}`

Goal must link to exactly one of: Asset or AssetGroup (validated in steps).

Progress calculation: latest snapshot value for linked asset/group ÷ target value × 100%.

`GET /api/v1/goals` and `GET /api/v1/goals/{id}` include current progress in the response.

## Dependencies

TASK-03, TASK-06, TASK-07, TASK-10, TASK-11, TASK-13

## Status

Done
