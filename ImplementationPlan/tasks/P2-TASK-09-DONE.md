# TASK-09 — Account Feature (Setup Wizard)

## Description

Implement account endpoints.

`GET /api/v1/account` — return current account (currency, periodicity, start date, setup complete flag)

`PUT /api/v1/account` — update account setup fields (currency ISO 4217, snapshot start date, periodicity enum). Validates start date is not in the future.

Steps for PUT: Validate → Persist

## Dependencies

TASK-03, TASK-06, TASK-07

## Status

Done
