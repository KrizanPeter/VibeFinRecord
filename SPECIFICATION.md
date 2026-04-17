# FinClaude — Personal Finance Tracker
## Specification v0.1

---

## 1. Overview

FinClaude is a personal finance tracking application that gives users a clear, long-term view of their total wealth across all their assets. The core problem it solves: when you have many assets (bank accounts, savings, ETFs, pension funds, etc.) spread across multiple institutions, it is hard to know your actual net worth or whether your wealth is growing over time.

The app solves this by having users periodically snapshot the value of all their assets. Over time, these snapshots form a timeline that reveals trends, growth, and progress toward financial goals.

---

## 2. Technology Stack

| Layer         | Technology                                   |
|---------------|----------------------------------------------|
| Mobile App    | React Native (iOS & Android)                 |
| Backend API   | .NET (C#), RESTful HTTP API                  |
| Architecture  | Monolithic, Clean Architecture               |
| Database      | SQLite                                       |
| Auth          | ASP.NET Core Identity with JWT               |
| Future        | Web app (React or React Native Web)          |

### 2.1 Backend Architecture (Clean Architecture Layers)
- **Domain** — Entities, value objects, domain rules (no dependencies)
- **Application** — Use cases, DTOs, interfaces (depends on Domain only)
- **Infrastructure** — EF Core + SQLite, Identity, external services (implements Application interfaces)
- **API** — Controllers, middleware, DI composition root (depends on Application + Infrastructure)

---

## 3. Authentication & User Management

ASP.NET Core Identity is used **exclusively** for authentication concerns (credential storage, password hashing, JWT issuance). It has no role in runtime domain operations.

- Provider: **ASP.NET Core Identity**
- Token format: **JWT (Bearer token)**
- Refresh token storage: **ASP.NET Identity `UserTokens` table** via `UserManager.SetAuthenticationTokenAsync` / `GetAuthenticationTokenAsync`. Token name: `RefreshToken`; provider name: `FinClaude`. Stored hashed.
- Features in scope:
  - Register (email + password) → also creates a linked `Account` (see §4.1)
  - Login (returns JWT containing `AccountId`)
  - Refresh token (rotate on use — old token invalidated, new token issued)
- Features **out of scope** (MVP):
  - Roles and permissions
  - Social login (Google, Apple, etc.)
  - Password reset via email
  - Two-factor authentication

**Separation of concerns:**

| Layer          | Responsibility                                                        |
|----------------|-----------------------------------------------------------------------|
| Identity User  | Credential storage, password hashing, JWT issuance only              |
| Account        | All runtime operations: preferences, entity ownership, domain queries |

After registration the system automatically creates a corresponding `Account` record. The JWT includes `AccountId` as a claim. All authenticated API handlers resolve the current actor via `AccountId`, never via the Identity `UserId` directly.

**Account Setup guard:** A middleware checks on every authenticated request whether `Account.SnapshotStartDate` is null (setup incomplete). If so, all endpoints except `/api/v1/auth/*` and `PUT /api/v1/account` return `403 Forbidden` with a Problem Details body:

```json
{
  "type": "https://finclaude.app/errors/account-setup-required",
  "title": "Account setup is not complete.",
  "status": 403
}
```

The mobile app detects this response and redirects to the Account Setup wizard.

---

## 4. Core Entities

### 4.0 BaseEntity
All domain entities extend `BaseEntity`. Tables only list domain-specific fields — `Id`, `CreatedAt`, `UpdatedAt`, and `DeletedAt` are inherited.

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }

    public bool IsDeleted => DeletedAt.HasValue;
}
```

| Field      | Type      | Notes                                                              |
|------------|-----------|--------------------------------------------------------------------|
| Id         | UUID      | Primary key, set on creation                                       |
| CreatedAt  | datetime  | Set once on creation                                               |
| UpdatedAt  | datetime  | Updated on every save via EF Core interceptor or `SaveChanges` override |
| DeletedAt  | datetime? | Null = active. Set on soft-delete. Never physically removed.       |

**Soft-delete:** A global EF Core query filter (`HasQueryFilter(e => e.DeletedAt == null)`) is applied to all entities that support soft-delete. `AssetSnapshot` is excluded — it is an immutable historical record and is never soft-deleted.

---

### 4.1 Account
*Extends `BaseEntity`.*

The primary domain entity. Created automatically on registration and linked 1-to-1 with an Identity User. All other domain entities reference `Account`, not the Identity User.

| Field               | Type   | Required | Notes                                               |
|---------------------|--------|----------|-----------------------------------------------------|
| IdentityUserId      | string | Yes      | FK → `AspNetUsers.Id` (Identity), unique            |
| Currency            | string | Yes      | ISO 4217 currency code (e.g. `EUR`, `USD`, `CZK`)  |
| SnapshotStartDate   | date   | No       | Null until Account Setup wizard is completed        |
| SnapshotPeriodicity | enum   | No       | `Monthly` \| `Quarterly` \| `Yearly`; null until setup |

- `SnapshotStartDate` and `SnapshotPeriodicity` are null until the user completes Account Setup.
- The JWT `AccountId` claim is resolved from `IdentityUserId` on every authenticated request.

### 4.2 Asset
*Extends `BaseEntity`. Soft-delete applied.*

Represents a single financial asset owned by the account.

| Field       | Type   | Required | Notes                                 |
|-------------|--------|----------|---------------------------------------|
| AccountId   | UUID   | Yes      | Owner (FK → Account)                  |
| Name        | string | Yes      | e.g. "Raiffeisen Savings", "VWCE ETF" |
| Institution | string | No       | Name of the bank/broker/custodian     |

- An Asset is generic — the user defines what it represents.
- No asset type categorization enforced by the system.
- Soft-deleted assets are hidden from future snapshot forms but their `AssetSnapshot` records remain intact for historical calculations.

### 4.3 Asset Group
*Extends `BaseEntity`. Soft-delete applied.*

A user-defined label that groups assets together for aggregated tracking.

| Field     | Type   | Required | Notes                               |
|-----------|--------|----------|-------------------------------------|
| AccountId | UUID   | Yes      | Owner (FK → Account)                |
| Name      | string | Yes      | e.g. "Retirement", "Emergency Fund" |

- An Asset can belong to **zero or more** Groups.
- A Group can contain **zero or more** Assets.
- Relationship: many-to-many (`AssetGroupMembership` join table — see below).

**AssetGroupMembership** (join table, no `BaseEntity`):

| Field       | Type | Required | Notes          |
|-------------|------|----------|----------------|
| AssetId     | UUID | Yes      | FK → Asset     |
| GroupId     | UUID | Yes      | FK → AssetGroup |

Composite primary key: `(AssetId, GroupId)`.

### 4.4 Snapshot
*Extends `BaseEntity`. Soft-delete applied.*

A point-in-time recording of the value of every asset at a given snapshot date.

| Field        | Type | Required | Notes                                     |
|--------------|------|----------|-------------------------------------------|
| AccountId    | UUID | Yes      | Owner (FK → Account)                      |
| SnapshotDate | date | Yes      | The date this snapshot represents         |

### 4.5 AssetSnapshot
*Extends `BaseEntity`. Soft-delete **not** applied — immutable historical record.*

The value of a single asset within a Snapshot.

| Field      | Type          | Required | Notes                                        |
|------------|---------------|----------|----------------------------------------------|
| SnapshotId | UUID          | Yes      | FK → Snapshot                                |
| AssetId    | UUID          | Yes      | FK → Asset                                   |
| Value      | decimal(18,4) | Yes      | Value of the asset on that date              |

- Every Snapshot must include a value for every active (non-soft-deleted) Asset belonging to the account at the time of submission.
- `decimal(18,4)` — four decimal places for financial precision.

### 4.6 Goal
*Extends `BaseEntity`. Soft-delete applied.*

A target value that the user wants an Asset or Asset Group to reach by a specific date.

| Field       | Type          | Required | Notes                                |
|-------------|---------------|----------|--------------------------------------|
| AccountId   | UUID          | Yes      | Owner (FK → Account)                 |
| Name        | string        | Yes      | e.g. "Retirement nest egg"           |
| TargetValue | decimal(18,4) | Yes      | The monetary target                  |
| TargetDate  | date          | Yes      | Deadline to reach the target         |
| AssetId     | UUID          | No       | Linked to a specific Asset (or null) |
| GroupId     | UUID          | No       | Linked to an Asset Group (or null)   |

- A Goal must be linked to exactly one of: an Asset OR an Asset Group (not both, not neither).
- Progress is calculated as: `currentValue / targetValue × 100%` where `currentValue` is the latest snapshot value for the linked Asset or Group.
- When the linked Asset or AssetGroup is soft-deleted, all associated Goals are cascade soft-deleted.

---

## 5. Use Cases

### 5.1 Register
1. User provides email and password.
2. System creates an Identity User record.
3. System automatically creates a linked `Account` record (profile fields null until setup).
4. System returns JWT containing `AccountId`.
5. App redirects to Profile Setup.

### 5.2 Login
1. User provides email and password.
2. System validates credentials and returns JWT + refresh token.
3. **After login**, system runs Snapshot Gap Detection (see §5.4).

### 5.3 Account Setup (first login)
1. User selects base currency.
2. User picks snapshot start date (cannot be in the future).
3. User picks snapshot periodicity: `Monthly` | `Quarterly` | `Yearly`.
4. `Account` record is updated with these values.

**Post-setup field edit rules** (applies to `PUT /api/v1/account` after setup is complete):
- `Currency` — editable freely at any time. Only affects display formatting; does not affect historical values.
- `SnapshotStartDate` and `SnapshotPeriodicity` — **locked** once any `Snapshot` exists for the account. Attempting to change them after the first snapshot returns `409 Conflict`. If no snapshots exist yet, they can be changed freely.

### 5.4 Snapshot Gap Detection (automatic, post-login)
After every login the backend calculates which snapshot dates are expected but missing:

```
expectedDates = generate dates from startDate to today using periodicity
missingSnapshotDates = expectedDates - existingSnapshotDates
```

If `missingSnapshotDates` is not empty, the app **blocks navigation** and requires the user to fill all missing snapshots before accessing any other screen (oldest first). There is no skip option — gaps in the timeline would corrupt net worth trends and goal progress calculations.

### 5.5 Take a Snapshot
1. App presents a form showing all of the user's assets.
2. For each asset, the user enters the current value.
3. User submits. A Snapshot + AssetSnapshots are created for the target date.
4. If filling in multiple missing snapshots, the flow repeats for each date.

**Snapshot date validation rules:**
- The submitted date must appear in the missing-dates list returned by gap detection. Dates that are not expected (not matching the periodicity schedule) are rejected with `400 Bad Request`.
- Future dates are rejected with `400 Bad Request` (gap detection never produces future dates, so this is a client-side guard).
- A date for which a `Snapshot` already exists is rejected with `409 Conflict`.

### 5.6 Manage Assets (CRUD)
- Create, read, update, and delete Assets.
- Soft-delete: deleting an asset hides it from future snapshot forms but preserves historical AssetSnapshot data.

### 5.7 Manage Asset Groups (CRUD)
- Create, read, update, and delete Asset Groups.
- Assign / remove Assets from a Group.

### 5.8 Manage Goals (CRUD)
- Create, read, update, and delete Goals.
- Link a Goal to an Asset or Asset Group.
- View goal progress (current value vs. target, percentage, projected completion based on trend).

### 5.9 Dashboard (future — not MVP)
- Net worth over time (line chart from snapshots).
- Per-group net worth over time.
- Goal progress bars.
- Growth rate per period (MoM, QoQ, YoY).
- Configurable widgets.

---

## 6. Snapshot Periodicity

Supported periods:

| Key         | Description          | Example dates (start: 2024-01-01)  |
|-------------|----------------------|------------------------------------|
| `Monthly`   | Same day each month  | Jan 1, Feb 1, Mar 1, …             |
| `Quarterly` | Every 3 months       | Jan 1, Apr 1, Jul 1, Oct 1, …      |
| `Yearly`    | Same day each year   | Jan 1, 2024; Jan 1, 2025; …        |

If the target day doesn't exist in a month, clamp to the last day of that month — applied independently each month (e.g. start Jan 31 → Feb 28 → Mar 31 → Apr 30 → May 31).

---

## 7. API Design Principles

- RESTful JSON API
- All endpoints require Bearer JWT except `/auth/register` and `/auth/login`
- Base path: `/api/v1/`
- Error responses follow RFC 9457 Problem Details format
- Pagination on list endpoints: **offset-based** (`?page=1&pageSize=20`). Response envelope: `{ items: [...], totalCount, page, pageSize }`.

### Endpoints

| Method   | Path                                    | Auth | Description                              |
|----------|-----------------------------------------|------|------------------------------------------|
| `POST`   | `/api/v1/auth/register`                 | No   | Register; creates Identity User + Account |
| `POST`   | `/api/v1/auth/login`                    | No   | Login; returns JWT + refresh token       |
| `POST`   | `/api/v1/auth/refresh`                  | No   | Exchange refresh token for new JWT       |
| `GET`    | `/api/v1/account`                       | Yes  | Get current account (currency, periodicity, setup status) |
| `PUT`    | `/api/v1/account`                       | Yes  | Update account setup (currency, start date, periodicity) |
| `GET`    | `/api/v1/assets`                        | Yes  | List active assets                       |
| `POST`   | `/api/v1/assets`                        | Yes  | Create asset                             |
| `GET`    | `/api/v1/assets/{id}`                   | Yes  | Get asset detail                         |
| `PUT`    | `/api/v1/assets/{id}`                   | Yes  | Update asset                             |
| `DELETE` | `/api/v1/assets/{id}`                   | Yes  | Soft-delete asset (cascades to Goals)    |
| `GET`    | `/api/v1/groups`                        | Yes  | List groups                              |
| `POST`   | `/api/v1/groups`                        | Yes  | Create group                             |
| `GET`    | `/api/v1/groups/{id}`                   | Yes  | Get group with member assets             |
| `PUT`    | `/api/v1/groups/{id}`                   | Yes  | Update group name                        |
| `DELETE` | `/api/v1/groups/{id}`                   | Yes  | Soft-delete group (cascades to Goals)    |
| `POST`   | `/api/v1/groups/{id}/assets`            | Yes  | Add asset to group                       |
| `DELETE` | `/api/v1/groups/{id}/assets/{assetId}`  | Yes  | Remove asset from group                  |
| `GET`    | `/api/v1/snapshots/status`              | Yes  | Get missing snapshot dates + next due date |
| `GET`    | `/api/v1/snapshots`                     | Yes  | List snapshots                           |
| `POST`   | `/api/v1/snapshots`                     | Yes  | Submit a snapshot (all asset values)     |
| `GET`    | `/api/v1/snapshots/{id}`                | Yes  | Get snapshot with all asset values       |
| `GET`    | `/api/v1/goals`                         | Yes  | List goals with current progress         |
| `POST`   | `/api/v1/goals`                         | Yes  | Create goal                              |
| `GET`    | `/api/v1/goals/{id}`                    | Yes  | Get goal with progress detail            |
| `PUT`    | `/api/v1/goals/{id}`                    | Yes  | Update goal                              |
| `DELETE` | `/api/v1/goals/{id}`                    | Yes  | Soft-delete goal                         |

---

## 8. Mobile App Structure (React Native)

### 8.0 Libraries

| Concern      | Library                          | Notes                                                              |
|--------------|----------------------------------|--------------------------------------------------------------------|
| Navigation   | `React Navigation`               | Stack + bottom tab navigators matching the navigation structure    |
| HTTP client  | `Axios`                          | Interceptors for JWT injection and automatic refresh token handling |
| Server state | `TanStack Query (React Query)`   | Caching, refetching, loading/error states for all API calls        |
| Local state  | `Zustand`                        | Auth state, account setup completion flag, lightweight global state |
| Forms        | `React Hook Form`                | Snapshot entry form (many fields), asset/goal forms                |

### Screens

| Screen              | Purpose                                               |
|---------------------|-------------------------------------------------------|
| Login / Register    | Authentication                                        |
| Account Setup       | Currency, start date, periodicity (first-time wizard) |
| Snapshot Gate       | Enforced post-login flow — all missing snapshots must be filled before proceeding |
| Snapshot Entry Form | Enter values for all assets for a given snapshot date |
| Assets List         | View, create, edit, delete assets                     |
| Asset Detail        | History of values for one asset                       |
| Groups List         | View, create, edit, delete groups; assign assets      |
| Goals List          | View all goals with progress                          |
| Goal Detail / Form  | Create or edit a goal                                 |
| Dashboard           | (Future) Charts and summary widgets                   |

### Navigation Structure

```
Root
├── Auth Stack (unauthenticated)
│   ├── Login
│   └── Register
└── App Stack (authenticated)
    ├── Account Setup (wizard, shown once)
    ├── Snapshot Gate (blocks navigation until all missing snapshots are filled)
    └── Bottom Tab Navigator
        ├── Dashboard (future)
        ├── Assets
        ├── Groups
        └── Goals
```

---

## 9. Out of Scope (MVP)

The following are explicitly deferred to future releases:

1. **Email reminders** — Tokenized snapshot form sent by email when a snapshot is due. (Architecture should be designed to add this without major changes.)
2. **Web app** — A web version of the app using the same API.
3. **Bank/broker API integrations** — Automatic pulling of asset values.
4. **Multiple currencies** — All values in a single base currency.
5. **Roles and authorization** — Only one role (authenticated user) in MVP.
6. **Dashboard charts** — UI for visual analytics.
7. **Social / OAuth login** — Only email/password in MVP.

---

## 10. Non-Functional Requirements

| Concern        | Requirement                                                                         |
|----------------|-------------------------------------------------------------------------------------|
| Security       | Passwords hashed with ASP.NET Core Identity (PBKDF2). JWT expiry: **24h access / 30 days refresh** (development). Production target: 15 min access / 30 days refresh. Axios interceptor handles silent refresh on 401. |
| Data isolation | All domain queries scoped to `AccountId` resolved from the JWT claim.               |
| Offline        | Not required for MVP. App requires connectivity.                                    |
| Platforms      | iOS and Android via React Native.                                                   |
| Database       | SQLite via Entity Framework Core; migrations managed with EF tooling.               |
| Logging        | Structured logging (Serilog or built-in .NET logging) to file/console.              |

---

## 11. .NET Backend Rules

These rules apply to the entire backend and must be followed consistently across all features.

---

### 11.1 CQRS — Commands and Queries

The application layer is split into **Commands** (write intent) and **Queries** (read intent). No shared base class; they are distinct by design.

**Library:** Hand-rolled dispatcher — no MediatR or paid alternatives. Define minimal interfaces directly:

```csharp
// Application layer interfaces
public interface ICommand { }
public interface ICommand<TResult> { }
public interface IQuery<TResult> { }

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task<ErrorOr<Success>> HandleAsync(TCommand command, CancellationToken ct);
}

public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<ErrorOr<TResult>> HandleAsync(TCommand command, CancellationToken ct);
}

public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<ErrorOr<TResult>> HandleAsync(TQuery query, CancellationToken ct);
}
```

Handlers are registered in DI and resolved directly by controllers — no reflection-based dispatcher is needed for a monolith of this scale.

> **Alternative considered:** `Mediator` by martinothamar (NuGet: `Mediator`) — source-generated, zero reflection, free, near drop-in MediatR replacement. Acceptable if hand-rolled feels too verbose.

---

### 11.2 Controllers — Thin Dispatchers

Controllers contain **no logic**. Their only responsibility is:
1. Extract and validate the incoming HTTP request into a command or query object.
2. Resolve and invoke the appropriate handler.
3. Map the handler result to an HTTP response.

```csharp
[ApiController]
[Route("api/v1/assets")]
public class AssetsController : ControllerBase
{
    private readonly ICommandHandler<CreateAssetCommand, Guid> _createHandler;

    public AssetsController(ICommandHandler<CreateAssetCommand, Guid> createHandler)
        => _createHandler = createHandler;

    [HttpPost]
    public async Task<IActionResult> Create(CreateAssetRequest request, CancellationToken ct)
    {
        var command = new CreateAssetCommand(AccountId, request.Name, request.Institution);
        var result = await _createHandler.HandleAsync(command, ct);
        return result.IsSuccess ? CreatedAtAction(...) : Problem(result);
    }
}
```

Rules:
- No business logic, no repository calls, no `if/else` beyond HTTP mapping.
- `AccountId` is extracted from the JWT claim via a base controller helper or extension method.

---

### 11.3 Handlers — Orchestration Only

Handlers are **orchestrators**, not implementors of business logic. A handler:
- Injects `IUnitOfWork` and an `IChainProvider<TContext>`.
- Builds a context from the incoming command/query.
- Retrieves the pre-built chain from the provider and executes the first step.
- Wraps the execution in a Unit of Work.
- Does not contain domain decisions itself and never loops over steps.

**`IChainProvider<TContext>`** is a per-command interface whose implementation wires up the full step chain and returns the first step (see §11.4):

```csharp
public interface IChainProvider<TContext>
{
    IStep<TContext> GetChain();
}
```

**Handler example:**

```csharp
public class CreateAssetCommandHandler : ICommandHandler<CreateAssetCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly IChainProvider<CreateAssetContext> _chainProvider;

    public CreateAssetCommandHandler(
        IUnitOfWork uow,
        IChainProvider<CreateAssetContext> chainProvider)
    {
        _uow = uow;
        _chainProvider = chainProvider;
    }

    public async Task<ErrorOr<Guid>> HandleAsync(CreateAssetCommand command, CancellationToken ct)
    {
        var context = new CreateAssetContext(command);
        var chain = _chainProvider.GetChain();

        await _uow.BeginAsync(ct);
        try
        {
            var result = await chain.ExecuteAsync(context, ct);
            if (result.IsError) { await _uow.RollbackAsync(ct); return result.Errors; }

            await _uow.CommitAsync(ct);
            return context.CreatedId;
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }
}
```

---

### 11.4 Chain of Responsibility — Step Pipeline

Business logic inside a handler is broken into **discrete, single-responsibility steps** chained together like middleware. Each step holds a reference to `_next` and decides whether to invoke it — the handler does not loop over steps externally.

```csharp
public interface IStep<TContext>
{
    Task<ErrorOr<Success>> ExecuteAsync(TContext context, CancellationToken ct);
}
```

A `TContext` object is a mutable bag that carries the command input and accumulates output (e.g., the newly created entity's ID) across steps.

**Query step result propagation:** Query context objects carry a typed `Result` property that the final step populates. The handler reads it after chain execution:

```csharp
public class GetAssetContext
{
    public GetAssetQuery Query { get; }
    public AssetDto? Result { get; set; }  // populated by the final query step

    public GetAssetContext(GetAssetQuery query) => Query = query;
}

// In the handler — after chain.ExecuteAsync:
var result = await chain.ExecuteAsync(context, ct);
if (result.IsError) return result.Errors;
return context.Result!;
```

This keeps the step interface uniform (`IStep<TContext>` always returns `ErrorOr<Success>`) for both command and query chains.

**All steps inherit from `BaseStep<TContext>` and call `NextAsync` to continue the chain:**

```csharp
public class ValidateAssetStep : BaseStep<CreateAssetContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateAssetContext context, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(context.Command.Name))
            return Error.Validation("Asset.Name", "Name is required.");

        return await NextAsync(context, ct);
    }
}

// Terminal step — calls NextAsync which returns Result.Success when _next is null
public class PersistAssetStep : BaseStep<CreateAssetContext>
{
    private readonly IAssetRepository _repository;

    public PersistAssetStep(IAssetRepository repository) => _repository = repository;

    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateAssetContext context, CancellationToken ct)
    {
        await _repository.AddAsync(context.Asset, ct);
        return await NextAsync(context, ct);
    }
}
```

**Chain is built in the `IChainProvider` implementation and returned as the first step:**

```csharp
public class CreateAssetChainProvider : IChainProvider<CreateAssetContext>
{
    // Steps are injected (resolved from DI) so they can have their own dependencies
    private readonly ValidateAssetStep _validate;
    private readonly AuthorizeAssetStep _authorize;
    private readonly CreateAssetDomainStep _domain;
    private readonly PersistAssetStep _persist;

    public CreateAssetChainProvider(
        ValidateAssetStep validate,
        AuthorizeAssetStep authorize,
        CreateAssetDomainStep domain,
        PersistAssetStep persist)
    {
        _validate = validate;
        _authorize = authorize;
        _domain = domain;
        _persist = persist;
    }

    public IStep<CreateAssetContext> GetChain()
    {
        _validate.SetNext(_authorize);
        _authorize.SetNext(_domain);
        _domain.SetNext(_persist);
        // _persist is terminal — no SetNext call
        return _validate;
    }
}
```

Steps expose `SetNext` rather than receiving `_next` through their constructor, keeping DI-friendly construction separate from chain wiring:

```csharp
public abstract class BaseStep<TContext> : IStep<TContext>
{
    private IStep<TContext>? _next;

    public void SetNext(IStep<TContext> next) => _next = next;

    protected Task<ErrorOr<Success>> NextAsync(TContext context, CancellationToken ct)
        => _next?.ExecuteAsync(context, ct) ?? Task.FromResult(Result.Success);

    public abstract Task<ErrorOr<Success>> ExecuteAsync(TContext context, CancellationToken ct);
}
```

**Typical step order for a command handler:**

| Order | Step                 | Responsibility                                          |
|-------|----------------------|---------------------------------------------------------|
| 1     | Validation step      | Validate command fields                                 |
| 2     | Authorization step   | Verify the Account owns all referenced resources        |
| 3     | Domain logic step(s) | Create/mutate domain entities                           |
| 4     | Persistence step     | Write to repository via `_next`-terminal               |

Rules:
- Each step is independently testable.
- A step short-circuits by returning `Error.*` without calling `_next`.
- A step can execute logic both **before** and **after** calling `_next` (e.g., for logging or enrichment).
- Query handler steps follow the same pattern but are read-only (no UoW, no mutations).

---

### 11.5 Unit of Work

- The `IUnitOfWork` interface lives in the **Application** layer.
- Its implementation (wrapping EF Core `DbContext.SaveChangesAsync`) lives in **Infrastructure**.
- UoW is **opened and committed/rolled back exclusively in the handler** — never inside a step or repository.
- Repositories within a step call `Add`, `Update`, `Remove` on the EF context but do **not** call `SaveChangesAsync` themselves.

```csharp
// Application/Interfaces
public interface IUnitOfWork
{
    Task BeginAsync(CancellationToken ct);
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
}
```

---

### 11.6 Result Pattern

All handler and step return types use **`ErrorOr<T>`** (NuGet: `ErrorOr`). No exceptions for expected domain failures.

```csharp
// Command with a return value
Task<ErrorOr<Guid>> HandleAsync(CreateAssetCommand command, CancellationToken ct);

// Command with no return value
Task<ErrorOr<Success>> HandleAsync(DeleteAssetCommand command, CancellationToken ct);

// Query
Task<ErrorOr<AssetDto>> HandleAsync(GetAssetQuery query, CancellationToken ct);

// Step
Task<ErrorOr<Success>> ExecuteAsync(CreateAssetContext context, CancellationToken ct);
```

**Why `ErrorOr`:**
- Built-in `Error` categories (`Error.Validation`, `Error.NotFound`, `Error.Conflict`, `Error.Unexpected`) map directly to RFC 9457 Problem Details HTTP responses (§7).
- `ErrorOr<Success>` covers void operations without a hand-rolled unit type.
- Chain short-circuit is idiomatic: `if (result.IsError) return result.Errors;`
- Single lightweight NuGet package, no transitive dependencies.

Controllers map `ErrorOr` results to HTTP responses via a shared extension method or base controller helper — keeping the mapping logic in one place.

---

## 12. Testing Strategy

| Layer       | What to test                                              | How                                                      |
|-------------|-----------------------------------------------------------|----------------------------------------------------------|
| Steps       | Each step in isolation — happy path + all error cases     | Unit tests; mock injected dependencies (repositories etc.) |
| Handlers    | Orchestration — UoW opens, commits, and rolls back correctly | Unit tests; mock `IChainProvider` and `IUnitOfWork`    |
| Domain      | Business rules on entities and value objects              | Unit tests; no mocks needed                              |
| API         | Request → response mapping, auth enforcement, Problem Details format | Integration tests via `WebApplicationFactory`  |
| Repositories| EF Core queries return correct filtered/scoped data       | Integration tests against a real SQLite test database    |

Rules:
- Steps must be the primary unit test target — each step is a single, focused behaviour and should be tested independently.
- No mocking the database in repository tests — use a real SQLite instance to avoid mock/prod divergence.
- Integration tests own the full request pipeline (middleware, auth, mapping) — do not duplicate this in unit tests.

---

## 13. Open Questions / Future Decisions

- **Snapshot day alignment**: ✅ Resolved — when the target day does not exist in a month, clamp to the last day of that month (e.g. Jan 31 → Feb 28 → Mar 31 → Apr 30). This applies every month independently, always preserving end-of-month intent.
- **Asset soft-delete**: ✅ Resolved — soft-deleted assets are excluded from future snapshot forms but their `AssetSnapshot` records are always included in historical net worth calculations. History is never rewritten.
- **Goal linked to deleted asset/group**: ✅ Resolved — when an Asset or AssetGroup is soft-deleted, all Goals linked to it are cascade soft-deleted automatically. No orphaned or broken goal state to handle in the UI.
- **Multiple missing snapshots**: ✅ Resolved — enforced. If missing snapshots exist the user must fill all of them before accessing the rest of the app. Gaps in the timeline corrupt net worth trends and goal progress calculations.
- **Refresh token storage**: ✅ Resolved — stored in ASP.NET Identity `UserTokens` table via `UserManager`. Tokens are rotated on use. See §3.
- **Account setup field editability**: ✅ Resolved — `Currency` is freely editable. `SnapshotStartDate` and `SnapshotPeriodicity` are locked once any `Snapshot` exists (409 on attempt). See §5.3.
- **Snapshot date validation**: ✅ Resolved — only dates in the gap-detection missing list are accepted; future dates → 400; duplicate dates → 409. See §5.5.
- **Pagination strategy**: ✅ Resolved — offset-based pagination (`?page=1&pageSize=20`) on all list endpoints. See §7.
- **Query step result propagation**: ✅ Resolved — query contexts carry a typed `Result` property populated by the final step; handler reads it after chain execution. See §11.4.
- **Goal progress with no snapshots**: ✅ Resolved — return `currentValue: null` and `progressPercent: null` in the DTO when no snapshot data exists yet.
- **AssetGroupMembership on asset soft-delete**: ✅ Resolved — when an Asset is soft-deleted, its `AssetGroupMembership` rows are hard-deleted (cascade delete configured in EF). The join table has no soft-delete.
