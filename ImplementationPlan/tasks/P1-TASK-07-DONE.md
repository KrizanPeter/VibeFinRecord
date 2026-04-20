# TASK-07 — Account Setup Guard Middleware

## Description

Implement middleware in `FinClaude.Api` that enforces Account Setup completion.

- On every authenticated request, resolve `AccountId` from JWT claim
- Query `Account.SnapshotStartDate` — if null, return `403 Forbidden` (RFC 9457 Problem Details)
- Bypass for: all `/api/v1/auth/*` routes and `PUT /api/v1/account`
- `ErrorOr` → HTTP status mapping helper (shared base controller or extension method):
  - `Error.Validation` → 422
  - `Error.NotFound` → 404
  - `Error.Conflict` → 409
  - `Error.Unexpected` → 500

## Dependencies

TASK-05

## Status

Done
