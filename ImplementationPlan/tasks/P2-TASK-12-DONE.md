# TASK-12 — Snapshots Feature (Gap Detection)

## Description

Implement snapshot status and gap detection logic.

`GET /api/v1/snapshots/status`
- Generate all expected snapshot dates from `Account.SnapshotStartDate` using `SnapshotPeriodicity`
- Day-clamping rule: if day doesn't exist in month, use last day of that month (applied per month independently)
- Subtract existing snapshot dates
- Return: list of missing dates (ordered oldest first) + next expected date

## Dependencies

TASK-03, TASK-06, TASK-07, TASK-09

## Status

Done
