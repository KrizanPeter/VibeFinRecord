# TASK-30 — Dashboard Screen + Add Chart Wizard

## Description

Implement the Dashboard home tab and the Add Chart wizard.

**Dashboard screen:**
- Net-worth strip: total value from latest snapshot + delta vs. previous snapshot
- Scrollable list of chart cards — each shows: mini chart preview, source name, chart type chip, latest value
- TanStack Query for `GET /api/v1/dashboard/charts` + per-card `GET /api/v1/dashboard/charts/{id}/data`
- Empty state when no charts exist, prompting user to add the first chart
- FAB to launch Add Chart wizard

**Add Chart wizard** (4 steps, stack navigation):
1. Select source type: Asset or Asset Group
2. Select specific Asset or Group from a list
3. Select chart type (`Pie` only offered when source is a Group)
4. Set chart name (pre-filled with sensible default). Submit → `POST /api/v1/dashboard/charts`

## Dependencies

TASK-19, TASK-29, TASK-24, TASK-25

## Status

Waiting
