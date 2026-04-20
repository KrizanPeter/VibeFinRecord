# TASK-15 — Backend Integration Tests Setup

## Description

Set up `FinClaude.Integration.Tests` project using `WebApplicationFactory`.

- Test database: real SQLite (in-memory or temp file per test run)
- Auth helpers: generate valid JWTs for test accounts
- Seed helpers: create test accounts, assets, groups
- Verify: `AccountId` scoping (no cross-account data leakage), soft-delete filter, guard middleware returns 403 correctly

## Dependencies

TASK-08, TASK-09, TASK-10, TASK-11, TASK-13, TASK-14

## Status

Waiting
