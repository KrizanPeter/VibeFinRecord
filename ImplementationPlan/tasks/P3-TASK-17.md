# TASK-17 — Axios Client + JWT Interceptor

## Description

Set up the Axios HTTP client in `src/api`.

- Base URL from environment config
- Request interceptor: attach `Authorization: Bearer <token>` from Zustand auth store
- Response interceptor: on `401`, call `POST /api/v1/auth/refresh` transparently, retry original request
- On `403` with `account-setup-required` type: redirect to Account Setup screen
- Typed API functions per endpoint group (`authApi`, `accountApi`, `assetsApi`, etc.)

## Dependencies

TASK-16

## Status

Waiting
