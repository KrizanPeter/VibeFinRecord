# TASK-28 — Backend Integration Tests

## Description

Implement integration tests in `FinClaude.Integration.Tests` against the full API pipeline.

Coverage:
- Auth flow: register → login → refresh
- Guard middleware: 403 before setup, 200 after setup
- Snapshot Gate enforcement: missing snapshots block until filled
- Cross-account isolation: account A cannot access account B's data
- Soft-delete: deleted assets absent from lists, present in historical snapshots
- Cascade soft-delete: deleting asset/group removes linked goals

## Dependencies

TASK-15, TASK-27

## Status

Waiting
