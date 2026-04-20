# FinClaude — Product Specification (Lightweight)

## 1. Purpose
FinClaude is a personal finance tracker. Users periodically enter the value of their financial assets. The app builds a historical net‑worth timeline, visualizes trends, and tracks progress toward financial goals.

---

## 2. Core Concepts

### Account
Represents the user’s financial profile.
Stores:
- Base currency
- Snapshot start date
- Snapshot periodicity (Monthly / Quarterly / Yearly)

### Asset
Any financial item the user owns (bank account, ETF, pension fund, etc.).
Users can create, edit, and remove assets.

### Asset Group
User-defined grouping of assets (e.g., “Retirement”, “Savings”).
Assets may belong to multiple groups.

### Snapshot
A dated record containing the value of all active assets.
Forms the basis of the net‑worth timeline.

### AssetSnapshot
The value of a single asset within a snapshot.
Used for charts, goals, and net‑worth calculations.

### Goal
A target value for an Asset or Asset Group with a deadline.
Progress is calculated from the latest snapshot.

### DashboardChart
A user-created chart visualizing historical or current values of an Asset or Group.
Chart types: Line, Bar, Pie (Pie only for Groups).

---

## 3. Key Behaviors

### Snapshot Timeline
- User selects a start date and periodicity.
- The system determines expected snapshot dates.
- Missing snapshots must be filled in order from oldest to newest.
- Each snapshot contains values for all active assets.

### Net Worth
- Calculated from the latest snapshot.
- Shows change compared to the previous snapshot.

### Goals
- Users define target values and deadlines.
- Progress is calculated automatically from snapshot data.

### Dashboard
- Users build their own dashboard by adding charts.
- Each chart visualizes historical or latest values.

---

## 4. Use Cases

### Registration
User creates an account. A financial profile (Account) is created automatically.

### Login
User logs in. If snapshots are missing, they must be completed before continuing.

### Account Setup
User sets:
- Base currency
- Snapshot start date
- Snapshot periodicity

### Manage Assets
Users can create, edit, and remove assets.
Removed assets no longer appear in future snapshots, but historical data remains.

### Manage Asset Groups
Users can create groups, rename them, and assign or remove assets.

### Manage Snapshots
Users enter values for all assets for a specific date.
Snapshots form the net‑worth timeline.

### Manage Goals
Users can create, edit, and remove goals.
Progress is calculated automatically.

### Manage Dashboard Charts
Users can create charts (choose source, chart type, and name), view chart data, edit charts, and remove them.

---

## 5. Mobile App Structure

### Screens
- Login / Register
- Account Setup
- Snapshot Gate (fill missing snapshots)
- Snapshot Entry Form
- Dashboard (net worth + chart cards)
- Add Chart (wizard)
- Chart Detail
- Assets List / Detail / Form
- Groups List / Detail / Form
- Goals List / Detail / Form

### Navigation
Root
├── Auth Stack
└── App Stack
    ├── Account Setup
    ├── Snapshot Gate
    └── Tabs: Dashboard / Assets / Groups / Goals

---

## 6. Out of Scope (MVP)
- Email reminders
- Web version
- Automatic bank integrations
- Multi‑currency support
- Roles/permissions
- Social login
- Dashboard chart reordering
- Chart annotations

---

## 7. Non‑Functional Overview
- Data is isolated per user account
- App requires online connectivity
- Supports iOS and Android
- Logging available for diagnostics
