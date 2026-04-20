# TASK-23 — Snapshot Entry Form Screen

## Description

Implement the snapshot entry form for a single snapshot date.

Fetches the list of active assets. Renders one numeric input per asset (label = asset name + institution if set). Validates all fields are filled with a positive number. On submit: `POST /api/v1/snapshots`.

Used both from Snapshot Gate (TASK-22) and as a standalone "Add Snapshot" action.

## Dependencies

TASK-19, TASK-13, TASK-10

## Status

Waiting
