# FinClaude — Claude Code Guide

## What this project is

Personal finance tracker. Users snapshot the value of all their assets periodically; the app builds a net-worth timeline and tracks progress toward financial goals. Full spec: `SPECIFICATION.md`.

---

## Repository layout

```
FinClaude/
├── SPECIFICATION.md          — source of truth for all architectural decisions
├── IMPLEMENTATION_PLAN.md    — 31 tasks across 5 phases; check before starting work
├── design/                   — HTML/CSS mockups and design tokens (mobile screens)
│   ├── assets/tokens.css     — canonical design token values
│   ├── style-guide/          — component patterns and design principles
│   ├── screens/              — per-screen mockups (auth, dashboard, assets, etc.)
│   └── branding/             — logo, wordmark, brand usage rules
├── .claude/agents/           — sub-agents (see below)
└── [src not yet created]     — backend (.NET) and mobile (React Native) code
```

---

## Tech stack

| Layer | Technology |
|---|---|
| Mobile | React Native (iOS + Android) |
| Backend API | .NET (C#), RESTful, Clean Architecture |
| Database | SQLite via Entity Framework Core |
| Auth | ASP.NET Core Identity + JWT |

### Backend projects (Clean Architecture)

| Project | Layer | Depends on |
|---|---|---|
| `FinClaude.Domain` | Domain | nothing |
| `FinClaude.Application` | Application | Domain only |
| `FinClaude.Infrastructure` | Infrastructure | Application + Domain |
| `FinClaude.Api` | API | Application + Infrastructure |

### Mobile libraries

| Concern | Library |
|---|---|
| Navigation | React Navigation (Stack + Bottom Tabs) |
| HTTP | Axios (interceptors for JWT inject + silent refresh) |
| Server state | TanStack Query |
| Global state | Zustand |
| Forms | React Hook Form |

---

## Backend architecture rules (§11 of SPECIFICATION.md)

These apply everywhere in the backend. Violations are caught by the `code-reviewer` agent.

**Controllers** are thin dispatchers — extract request → call handler → map result to HTTP. No logic.

**Handlers** are orchestrators — build context, get chain from `IChainProvider`, execute, wrap in UoW. No business logic.

**Steps** (Chain of Responsibility) hold all business logic. Each step has one responsibility. Standard order: `Validate → Authorize → Domain logic → Persist`. Steps extend `BaseStep<TContext>` and call `NextAsync` to continue or return `Error.*` to short-circuit.

**Unit of Work** — `BeginAsync` / `CommitAsync` / `RollbackAsync` called **only in the handler**, never in a step or repository.

**Result pattern** — all handlers and steps return `ErrorOr<T>`. No exceptions for expected failures. ErrorOr → HTTP: `Validation → 422`, `NotFound → 404`, `Conflict → 409`, `Unexpected → 500`.

**Data scoping** — every query filtered by `AccountId` from the JWT claim. Never from the request body.

**Soft-delete** — `Asset`, `AssetGroup`, `Snapshot`, `Goal`, `DashboardChart` are soft-deleted (set `DeletedAt`). `AssetSnapshot` is immutable — never deleted. `AssetGroupMembership` rows are hard-deleted. Cascade: deleting an Asset or AssetGroup cascade-soft-deletes linked Goals and DashboardCharts.

---

## Mobile architecture rules

- All API calls via TanStack Query (`useQuery` / `useMutation`) — no raw `useEffect` + `useState` for server data.
- Auth tokens and setup state live in Zustand stores (`authStore`, `accountStore`).
- All forms use React Hook Form — no per-field `useState`.
- JWT injection and `401` silent refresh handled in Axios interceptors in `src/api` only.
- Route guards live in `src/navigation`, not in individual screens.
- Numeric/financial values rendered in JetBrains Mono with `fontVariant: ['tabular-nums']`.

---

## Available agents

Invoke these from Claude Code when the task matches.

| Agent | When to use |
|---|---|
| `spec-guardian` | New architectural decision made, or an existing spec rule needs to change |
| `plan-reviewer` | Start of a session — find out what's ready to work on next |
| `feature-scaffold` | Starting a backend vertical slice — generates command/query/context/steps/handler/controller stubs |
| `design-consultant` | Implementing a mobile screen — translates design tokens + mockups to React Native guidance; also audits existing code for design deviations |
| `code-reviewer` | After completing a task — audits code against §11 backend rules and mobile patterns |

---

## Implementation plan quick reference

31 tasks across 5 phases. Check `IMPLEMENTATION_PLAN.md` for full detail and current status.

| Phase | Scope | Key tasks |
|---|---|---|
| 1 — Backend Foundation | Solution scaffolding, entities, CQRS infra, EF Core, Identity, UoW, guard middleware | TASK-01 → TASK-07 |
| 2 — Backend Features | Auth, Account, Assets, Groups, Snapshots, Goals, DashboardCharts | TASK-08 → TASK-14, TASK-29 |
| 3 — Mobile Foundation | RN project setup, Axios client, Zustand stores, navigation | TASK-16 → TASK-19 |
| 4 — Mobile Screens | Auth, Account Setup, Snapshot Gate, Assets, Groups, Goals, Dashboard | TASK-20 → TASK-26, TASK-30, TASK-31 |
| 5 — Testing | Integration test setup, unit tests, integration tests | TASK-15, TASK-27, TASK-28 |

---

## Design system

Design tokens and screen mockups are in `design/`. The canonical token file is `design/assets/tokens.css`.

- Base spacing grid: 4 px (`--sp-1` = 4 px … `--sp-10` = 64 px)
- Primary brand color: `#1F8A5E` (`--brand-500`)
- Radius scale: 6 / 10 / 14 / 20 / 999 px
- UI font: Inter; financial/numeric font: JetBrains Mono (tabular-nums)

Screen → mockup file mapping:

| Task(s) | Mockup |
|---|---|
| TASK-20 | `design/screens/auth.html` |
| TASK-21 | `design/screens/account-setup.html` |
| TASK-22/23 | `design/screens/snapshots.html` |
| TASK-24 | `design/screens/assets.html` |
| TASK-25 | `design/screens/groups.html` |
| TASK-26 | `design/screens/goals.html` |

---

## Key spec decisions (resolved)

- Refresh tokens stored in ASP.NET Identity `UserTokens` table, rotated on use
- `SnapshotStartDate` + `SnapshotPeriodicity` locked once any Snapshot exists (returns 409)
- Snapshot day clamping: if target day doesn't exist in a month, use last day of that month
- Missing snapshots: enforced in order, oldest first, no skip
- Pagination: offset-based (`?page=1&pageSize=20`), envelope `{ items, totalCount, page, pageSize }`
- `DashboardChart` Pie type only permitted with `SourceType = AssetGroup`
- Goal progress with no snapshots: return `currentValue: null, progressPercent: null`
