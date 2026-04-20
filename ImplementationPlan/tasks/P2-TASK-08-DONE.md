# TASK-08 — Auth Feature (Register / Login / Refresh)

## Description

Implement the three auth endpoints in `FinClaude.Api`.

`POST /api/v1/auth/register`
- Create Identity User
- Create linked `Account` record (setup fields null)
- Return JWT containing `AccountId`

`POST /api/v1/auth/login`
- Validate credentials
- Return JWT + refresh token

`POST /api/v1/auth/refresh`
- Validate refresh token
- Return new JWT + new refresh token

## Dependencies

TASK-05, TASK-06

## Status

Done
