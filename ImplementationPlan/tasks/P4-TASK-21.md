# TASK-21 — Account Setup Screen

## Description

Implement the Account Setup wizard screen.

Step 1: Currency selection (text input or picker, ISO 4217 code).
Step 2: Snapshot start date picker (cannot be future date).
Step 3: Periodicity selection (`Monthly` / `Quarterly` / `Yearly`).

On submit: `PUT /api/v1/account`. On success: update `accountStore.isSetupComplete` and navigate to Snapshot Gate check.

## Dependencies

TASK-19, TASK-09

## Status

Waiting
