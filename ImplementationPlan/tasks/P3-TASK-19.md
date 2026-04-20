# TASK-19 — Navigation Structure

## Description

Implement the full navigation tree in `src/navigation`.

```
Root
├── Auth Stack (unauthenticated)
│   ├── Login
│   └── Register
└── App Stack (authenticated)
    ├── Account Setup (wizard, shown when isSetupComplete = false)
    ├── Snapshot Gate (blocks until all missing snapshots filled)
    └── Bottom Tab Navigator
        ├── Dashboard   ← home tab; net-worth strip + user chart cards
        ├── Assets
        ├── Groups
        └── Goals
```

Route guards: check auth token → if missing, redirect to Auth Stack. Check setup complete → if not, redirect to Account Setup. Check snapshot status → if missing, redirect to Snapshot Gate.

## Dependencies

TASK-17, TASK-18

## Status

Waiting
