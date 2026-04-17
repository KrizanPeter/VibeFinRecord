# FinClaude — Implementation Plan

## Progress

**0 / 28 tasks completed**

| Phase | Tasks | Done |
|-------|-------|------|
| 1 — Backend Foundation | 7 | 0 |
| 2 — Backend Features | 8 | 0 |
| 3 — Mobile Foundation | 4 | 0 |
| 4 — Mobile Screens | 7 | 0 |
| 5 — Testing | 2 | 0 |

---

## Phase 1 — Backend Foundation

---

### TASK-01 — .NET Solution Scaffolding

**Description:** Create the .NET solution with four projects wired together following Clean Architecture dependency rules. Set up all NuGet packages needed across the solution.

Projects:
- `FinClaude.Domain` — no dependencies
- `FinClaude.Application` — references Domain
- `FinClaude.Infrastructure` — references Application + Domain
- `FinClaude.Api` — references Application + Infrastructure

Key packages:
- `ErrorOr` (Application, Domain)
- `Microsoft.EntityFrameworkCore.Sqlite` (Infrastructure)
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (Infrastructure)
- `Microsoft.AspNetCore.Authentication.JwtBearer` (Api)
- `Serilog.AspNetCore` (Api)

**Dependencies:** None

**Status:** Waiting

---

### TASK-02 — Domain Entities

**Description:** Implement `BaseEntity` and all six domain entities in `FinClaude.Domain`.

Entities: `Account`, `Asset`, `AssetGroup`, `AssetGroupMembership`, `Snapshot`, `AssetSnapshot`, `Goal`

Each entity extending `BaseEntity` gets: `Id`, `CreatedAt`, `UpdatedAt`, `DeletedAt`, `IsDeleted`.

Financial decimal fields (`AssetSnapshot.Value`, `Goal.TargetValue`) must be `decimal` typed — precision `(18,4)` is enforced at the EF configuration level (TASK-04).

**Dependencies:** TASK-01

**Status:** Waiting

---

### TASK-03 — CQRS + Chain of Responsibility Infrastructure

**Description:** Implement all architectural building blocks in `FinClaude.Application` and `FinClaude.Domain`.

Interfaces to create:
- `ICommand`, `ICommand<TResult>`, `IQuery<TResult>`
- `ICommandHandler<TCommand>` → `Task<ErrorOr<Success>>`
- `ICommandHandler<TCommand, TResult>` → `Task<ErrorOr<TResult>>`
- `IQueryHandler<TQuery, TResult>` → `Task<ErrorOr<TResult>>`
- `IStep<TContext>` → `Task<ErrorOr<Success>>`
- `IChainProvider<TContext>` → `IStep<TContext> GetChain()`
- `IUnitOfWork` → `BeginAsync`, `CommitAsync`, `RollbackAsync`

Base classes:
- `BaseStep<TContext>` — holds `_next`, exposes `SetNext`, `NextAsync`

**Dependencies:** TASK-01

**Status:** Waiting

---

### TASK-04 — EF Core + SQLite Setup

**Description:** Set up `AppDbContext` in `FinClaude.Infrastructure` with EF Core + SQLite.

- Entity type configurations for all entities (separate `IEntityTypeConfiguration<T>` classes)
- `decimal(18,4)` precision on all financial columns
- Global soft-delete query filter (`HasQueryFilter(e => e.DeletedAt == null)`) on: `Asset`, `AssetGroup`, `Snapshot`, `Goal`
- `AssetSnapshot` excluded from soft-delete filter (immutable historical record)
- `SaveChanges` / `SaveChangesAsync` override to auto-set `CreatedAt`, `UpdatedAt`
- Initial EF Core migration

**Dependencies:** TASK-02

**Status:** Waiting

---

### TASK-05 — ASP.NET Core Identity + JWT Setup

**Description:** Configure ASP.NET Core Identity and JWT authentication in `FinClaude.Infrastructure` and `FinClaude.Api`.

- `IdentityUser` setup (email/password only, no roles)
- JWT configuration: 24h access token expiry (dev), `AccountId` included as custom claim
- Refresh token storage and validation
- `ITokenService` interface (Application) + implementation (Infrastructure)
- DI registration in `Program.cs`

**Dependencies:** TASK-04

**Status:** Waiting

---

### TASK-06 — Unit of Work Implementation

**Description:** Implement `IUnitOfWork` in `FinClaude.Infrastructure` wrapping `AppDbContext`.

- `BeginAsync` — begins an EF Core transaction
- `CommitAsync` — calls `SaveChangesAsync` then commits transaction
- `RollbackAsync` — rolls back transaction
- Repositories must call only `Add`, `Update`, `Remove` on the context — never `SaveChangesAsync`
- Register `IUnitOfWork` in DI

**Dependencies:** TASK-04

**Status:** Waiting

---

### TASK-07 — Account Setup Guard Middleware

**Description:** Implement middleware in `FinClaude.Api` that enforces Account Setup completion.

- On every authenticated request, resolve `AccountId` from JWT claim
- Query `Account.SnapshotStartDate` — if null, return `403 Forbidden` (RFC 9457 Problem Details)
- Bypass for: all `/api/v1/auth/*` routes and `PUT /api/v1/account`
- `ErrorOr` → HTTP status mapping helper (shared base controller or extension method):
  - `Error.Validation` → 422
  - `Error.NotFound` → 404
  - `Error.Conflict` → 409
  - `Error.Unexpected` → 500

**Dependencies:** TASK-05

**Status:** Waiting

---

## Phase 2 — Backend Features

---

### TASK-08 — Auth Feature (Register / Login / Refresh)

**Description:** Implement the three auth endpoints in `FinClaude.Api`.

`POST /api/v1/auth/register`
- Create Identity User
- Create linked `Account` record (setup fields null)
- Return JWT containing `AccountId`

`POST /api/v1/auth/login`
- Validate credentials
- Return JWT + refresh token

`POST /api/v1/auth/refresh`
- Validate refresh token
- Return new JWT + new refresh token

**Dependencies:** TASK-05, TASK-06

**Status:** Waiting

---

### TASK-09 — Account Feature (Setup Wizard)

**Description:** Implement account endpoints.

`GET /api/v1/account` — return current account (currency, periodicity, start date, setup complete flag)

`PUT /api/v1/account` — update account setup fields (currency ISO 4217, snapshot start date, periodicity enum). Validates start date is not in the future.

Steps for PUT: Validate → Persist

**Dependencies:** TASK-03, TASK-06, TASK-07

**Status:** Waiting

---

### TASK-10 — Assets Feature (CRUD)

**Description:** Implement full asset management.

Endpoints: `GET /api/v1/assets`, `POST /api/v1/assets`, `GET /api/v1/assets/{id}`, `PUT /api/v1/assets/{id}`, `DELETE /api/v1/assets/{id}`

`DELETE` triggers soft-delete on the Asset and cascade soft-delete on all Goals linked to it.

Steps per command: Validate → Authorize (account owns asset) → Domain logic → Persist

**Dependencies:** TASK-03, TASK-06, TASK-07

**Status:** Waiting

---

### TASK-11 — Asset Groups Feature (CRUD + Member Management)

**Description:** Implement asset group management including member assignment.

Endpoints: `GET /api/v1/groups`, `POST /api/v1/groups`, `GET /api/v1/groups/{id}`, `PUT /api/v1/groups/{id}`, `DELETE /api/v1/groups/{id}`, `POST /api/v1/groups/{id}/assets`, `DELETE /api/v1/groups/{id}/assets/{assetId}`

`DELETE /api/v1/groups/{id}` triggers soft-delete on the group and cascade soft-delete on all Goals linked to it.

**Dependencies:** TASK-03, TASK-06, TASK-07, TASK-10

**Status:** Waiting

---

### TASK-12 — Snapshots Feature (Gap Detection)

**Description:** Implement snapshot status and gap detection logic.

`GET /api/v1/snapshots/status`
- Generate all expected snapshot dates from `Account.SnapshotStartDate` using `SnapshotPeriodicity`
- Day-clamping rule: if day doesn't exist in month, use last day of that month (applied per month independently)
- Subtract existing snapshot dates
- Return: list of missing dates (ordered oldest first) + next expected date

**Dependencies:** TASK-03, TASK-06, TASK-07, TASK-09

**Status:** Waiting

---

### TASK-13 — Snapshots Feature (Submit + List + Get)

**Description:** Implement snapshot submission and retrieval.

`POST /api/v1/snapshots` — accepts snapshot date + array of `{ assetId, value }` for all active assets. Validates all active assets are covered. Creates `Snapshot` + `AssetSnapshot` records.

`GET /api/v1/snapshots` — paginated list of past snapshots with total net worth per snapshot.

`GET /api/v1/snapshots/{id}` — single snapshot with all asset values.

**Dependencies:** TASK-12

**Status:** Waiting

---

### TASK-14 — Goals Feature (CRUD)

**Description:** Implement goal management with progress calculation.

Endpoints: `GET /api/v1/goals`, `POST /api/v1/goals`, `GET /api/v1/goals/{id}`, `PUT /api/v1/goals/{id}`, `DELETE /api/v1/goals/{id}`

Goal must link to exactly one of: Asset or AssetGroup (validated in steps).

Progress calculation: latest snapshot value for linked asset/group ÷ target value × 100%.

`GET /api/v1/goals` and `GET /api/v1/goals/{id}` include current progress in the response.

**Dependencies:** TASK-03, TASK-06, TASK-07, TASK-10, TASK-11, TASK-13

**Status:** Waiting

---

### TASK-15 — Backend Integration Tests Setup

**Description:** Set up `FinClaude.Integration.Tests` project using `WebApplicationFactory`.

- Test database: real SQLite (in-memory or temp file per test run)
- Auth helpers: generate valid JWTs for test accounts
- Seed helpers: create test accounts, assets, groups
- Verify: `AccountId` scoping (no cross-account data leakage), soft-delete filter, guard middleware returns 403 correctly

**Dependencies:** TASK-08, TASK-09, TASK-10, TASK-11, TASK-13, TASK-14

**Status:** Waiting

---

## Phase 3 — Mobile Foundation

---

### TASK-16 — React Native Project Setup

**Description:** Initialise the React Native project in `/mobile` and install all libraries.

- Framework: React Native (bare workflow or Expo)
- Libraries: `React Navigation` (stack + bottom tabs), `Axios`, `TanStack Query`, `Zustand`, `React Hook Form`
- TypeScript configuration
- Folder structure: `src/api`, `src/screens`, `src/navigation`, `src/store`, `src/hooks`, `src/components`, `src/types`
- Environment config for API base URL

**Dependencies:** None (parallel to backend)

**Status:** Waiting

---

### TASK-17 — Axios Client + JWT Interceptor

**Description:** Set up the Axios HTTP client in `src/api`.

- Base URL from environment config
- Request interceptor: attach `Authorization: Bearer <token>` from Zustand auth store
- Response interceptor: on `401`, call `POST /api/v1/auth/refresh` transparently, retry original request
- On `403` with `account-setup-required` type: redirect to Account Setup screen
- Typed API functions per endpoint group (`authApi`, `accountApi`, `assetsApi`, etc.)

**Dependencies:** TASK-16

**Status:** Waiting

---

### TASK-18 — Zustand Stores

**Description:** Implement global state stores in `src/store`.

`authStore`:
- `token`, `refreshToken`, `accountId`
- `setTokens`, `clearTokens`
- Persist to secure storage (device keychain)

`accountStore`:
- `isSetupComplete` (derived from account response)
- `currency`, `periodicity`

**Dependencies:** TASK-16

**Status:** Waiting

---

### TASK-19 — Navigation Structure

**Description:** Implement the full navigation tree in `src/navigation`.

```
Root
├── Auth Stack (unauthenticated)
│   ├── Login
│   └── Register
└── App Stack (authenticated)
    ├── Account Setup (wizard, shown when isSetupComplete = false)
    ├── Snapshot Gate (blocks until all missing snapshots filled)
    └── Bottom Tab Navigator
        ├── Assets
        ├── Groups
        └── Goals
```

Route guards: check auth token → if missing, redirect to Auth Stack. Check setup complete → if not, redirect to Account Setup. Check snapshot status → if missing, redirect to Snapshot Gate.

**Dependencies:** TASK-17, TASK-18

**Status:** Waiting

---

## Phase 4 — Mobile Screens

---

### TASK-20 — Auth Screens (Login + Register)

**Description:** Implement Login and Register screens.

Both use `React Hook Form` for form management. On success, store tokens in `authStore` and navigate to App Stack. Show field-level validation errors from API `422` responses.

**Dependencies:** TASK-19, TASK-08

**Status:** Waiting

---

### TASK-21 — Account Setup Screen

**Description:** Implement the Account Setup wizard screen.

Step 1: Currency selection (text input or picker, ISO 4217 code).
Step 2: Snapshot start date picker (cannot be future date).
Step 3: Periodicity selection (`Monthly` / `Quarterly` / `Yearly`).

On submit: `PUT /api/v1/account`. On success: update `accountStore.isSetupComplete` and navigate to Snapshot Gate check.

**Dependencies:** TASK-19, TASK-09

**Status:** Waiting

---

### TASK-22 — Snapshot Gate Screen

**Description:** Implement the enforced snapshot filling flow.

On mount: call `GET /api/v1/snapshots/status`. If missing dates exist, show the count and the oldest missing date. User cannot proceed until all are filled.

For each missing date: render Snapshot Entry Form (TASK-23). After each submission, advance to the next date. When all filled, navigate to Bottom Tab Navigator.

**Dependencies:** TASK-19, TASK-12, TASK-23

**Status:** Waiting

---

### TASK-23 — Snapshot Entry Form Screen

**Description:** Implement the snapshot entry form for a single snapshot date.

Fetches the list of active assets. Renders one numeric input per asset (label = asset name + institution if set). Validates all fields are filled with a positive number. On submit: `POST /api/v1/snapshots`.

Used both from Snapshot Gate (TASK-22) and as a standalone "Add Snapshot" action.

**Dependencies:** TASK-19, TASK-13, TASK-10

**Status:** Waiting

---

### TASK-24 — Assets Screens (List + Detail + Form)

**Description:** Implement three asset screens.

**Assets List** — `GET /api/v1/assets` via TanStack Query. Shows name + institution. FAB to create. Tap to detail.

**Asset Detail** — shows asset info + history of values from past snapshots (simple list, not chart). Edit and delete actions.

**Asset Form** (create + edit) — `React Hook Form`, fields: Name (required), Institution (optional).

**Dependencies:** TASK-19, TASK-10

**Status:** Waiting

---

### TASK-25 — Groups Screens (List + Detail + Form)

**Description:** Implement group management screens.

**Groups List** — list of groups. FAB to create.

**Group Detail** — shows group name + member assets. Add/remove asset membership via `POST|DELETE /api/v1/groups/{id}/assets`. Edit and delete group actions.

**Group Form** — single Name field.

**Dependencies:** TASK-19, TASK-11, TASK-24

**Status:** Waiting

---

### TASK-26 — Goals Screens (List + Detail + Form)

**Description:** Implement goal management screens.

**Goals List** — list of goals with inline progress bar (current % of target). TanStack Query.

**Goal Detail** — target value, target date, linked asset/group name, current value, progress percentage, projected completion date based on average growth trend.

**Goal Form** (create + edit) — Name, Target Value, Target Date, link to Asset or AssetGroup (toggle + picker).

**Dependencies:** TASK-19, TASK-14, TASK-24, TASK-25

**Status:** Waiting

---

## Phase 5 — Testing

---

### TASK-27 — Backend Unit Tests (Steps + Handlers)

**Description:** Implement unit tests in `FinClaude.Unit.Tests` targeting steps and handlers.

Per step: happy path + every possible error branch (each `Error.*` return).
Per handler: verify UoW opens + commits on success; verify UoW rolls back when chain returns error; verify UoW rolls back on exception.

Use xUnit + Moq (or NSubstitute) for mocking injected dependencies.

**Dependencies:** TASK-08 through TASK-14

**Status:** Waiting

---

### TASK-28 — Backend Integration Tests

**Description:** Implement integration tests in `FinClaude.Integration.Tests` against the full API pipeline.

Coverage:
- Auth flow: register → login → refresh
- Guard middleware: 403 before setup, 200 after setup
- Snapshot Gate enforcement: missing snapshots block until filled
- Cross-account isolation: account A cannot access account B's data
- Soft-delete: deleted assets absent from lists, present in historical snapshots
- Cascade soft-delete: deleting asset/group removes linked goals

**Dependencies:** TASK-15, TASK-27

**Status:** Waiting

---

*Last updated: 2026-04-18*
