# TASK-18 — Zustand Stores

## Description

Implement global state stores in `src/store`.

`authStore`:
- `token`, `refreshToken`, `accountId`
- `setTokens`, `clearTokens`
- Persist to secure storage (device keychain)

`accountStore`:
- `isSetupComplete` (derived from account response)
- `currency`, `periodicity`

## Dependencies

TASK-16

## Status

Waiting
