---
name: snapshot
description: Rules governing snapshot periodicity, gap detection, validation, and submission logic for the FinClaude backend. Ensures consistent timeline generation and data integrity across all snapshot operations.
---
## Snapshot Periodicity
- **Monthly**: same day each month.
- **Quarterly**: every 3 months from the start date.
- **Yearly**: same day each year.
- **Day‑clamping rule**:  
  If the target day does not exist in a given month (e.g., 31st), clamp to the **last day of that month**.  
  Applied independently for each month.

## Gap Detection Logic
- Generate expected snapshot dates from `SnapshotStartDate` up to **today** using the account's periodicity.
- Apply day‑clamping for each generated date.
- Subtract existing snapshot dates.
- Return:
  - List of **missing dates**, ordered from oldest to newest.
  - **Next expected date** (the next date after the most recent expected one).

## Snapshot Submission Rules
- Snapshot submission must include **all active (non‑soft‑deleted) assets**.
- Missing any asset → **422 Validation Error**.
- Submitted date must be one of the **missing dates** returned by gap detection.
- Future dates → **400 Bad Request**.
- If a snapshot already exists for the date → **409 Conflict**.

## Snapshot Data Rules
- A Snapshot contains:
  - `SnapshotDate`
  - A collection of `AssetSnapshot` entries.
- `AssetSnapshot.Value` uses **decimal(18,4)** precision.
- Historical data is immutable:
  - `AssetSnapshot` is **never** soft‑deleted.
  - Soft‑deleted assets remain visible in historical snapshots.
