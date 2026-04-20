# FinClaude — Dependency Graph

Format: `TASK ← [dependencies]`. Tasks with no deps can start immediately.

## Phase 1 — Backend Foundation
```
TASK-01  ← none
TASK-02  ← TASK-01
TASK-03  ← TASK-01
TASK-04  ← TASK-02
TASK-05  ← TASK-04
TASK-06  ← TASK-04
TASK-07  ← TASK-05
```

## Phase 2 — Backend Features
```
TASK-08  ← TASK-05, TASK-06
TASK-09  ← TASK-03, TASK-06, TASK-07
TASK-10  ← TASK-03, TASK-06, TASK-07
TASK-11  ← TASK-03, TASK-06, TASK-07, TASK-10
TASK-12  ← TASK-03, TASK-06, TASK-07, TASK-09
TASK-13  ← TASK-12
TASK-14  ← TASK-03, TASK-06, TASK-07, TASK-10, TASK-11, TASK-13
TASK-29  ← TASK-03, TASK-06, TASK-07, TASK-10, TASK-11, TASK-13
```

## Phase 3 — Mobile Foundation
```
TASK-16  ← none (parallel to backend)
TASK-17  ← TASK-16
TASK-18  ← TASK-16
TASK-19  ← TASK-17, TASK-18
```

## Phase 4 — Mobile Screens
```
TASK-20  ← TASK-19, TASK-08
TASK-21  ← TASK-19, TASK-09
TASK-22  ← TASK-19, TASK-12, TASK-23
TASK-23  ← TASK-19, TASK-10, TASK-13
TASK-24  ← TASK-19, TASK-10
TASK-25  ← TASK-19, TASK-11, TASK-24
TASK-26  ← TASK-19, TASK-14, TASK-24, TASK-25
TASK-30  ← TASK-19, TASK-24, TASK-25, TASK-29
TASK-31  ← TASK-30
```

## Phase 5 — Testing
```
TASK-15  ← TASK-08, TASK-09, TASK-10, TASK-11, TASK-13, TASK-14
TASK-27  ← TASK-08, TASK-09, TASK-10, TASK-11, TASK-12, TASK-13, TASK-14
TASK-28  ← TASK-15, TASK-27
```

## Critical path
```
TASK-01 → TASK-02 → TASK-04 → TASK-05 → TASK-07
                             → TASK-06
                                       → TASK-08 → TASK-20
                                       → TASK-09 → TASK-12 → TASK-13 → TASK-14 → TASK-29
                                       → TASK-10 → TASK-11             → TASK-26
                                                 → TASK-24 → TASK-25
TASK-16 → TASK-17 → TASK-19
        → TASK-18 ↗
```

