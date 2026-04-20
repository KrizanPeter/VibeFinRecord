# TASK-22 — Snapshot Gate Screen

## Description

Implement the enforced snapshot filling flow.

On mount: call `GET /api/v1/snapshots/status`. If missing dates exist, show the count and the oldest missing date. User cannot proceed until all are filled.

For each missing date: render Snapshot Entry Form (TASK-23). After each submission, advance to the next date. When all filled, navigate to Bottom Tab Navigator.

## Dependencies

TASK-19, TASK-12, TASK-23

## Status

Waiting
