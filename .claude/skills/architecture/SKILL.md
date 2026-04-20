---
name: architecture
description: Core architectural rules for the FinClaude backend. Ensures consistent Clean Architecture, CQRS, Step Pipeline, and Unit of Work behavior across all generated code.
---
## Clean Architecture Layers
- **Domain**: Entities, value objects, domain rules. No dependencies.
- **Application**: Commands, queries, DTOs, interfaces. Depends only on Domain.
- **Infrastructure**: EF Core, Identity, external services. Implements Application interfaces.
- **API**: Controllers, middleware, DI. Depends on Application + Infrastructure.

## CQRS Rules
- Commands represent write operations.
- Queries represent read operations.
- Handlers implement one of:
  - `ICommandHandler<TCommand>`
  - `ICommandHandler<TCommand, TResult>`
  - `IQueryHandler<TQuery, TResult>`

## Handler Rules
- Handlers contain **no business logic**.
- Build a context object.
- Resolve chain via `IChainProvider<TContext>`.
- Execute chain inside Unit of Work.
- On success → commit.
- On error or exception → rollback.

## Step Pipeline Rules
- Each step performs exactly **one** responsibility.
- Steps inherit from `BaseStep<TContext>`.
- Steps return `ErrorOr<Success>`.
- Steps call `NextAsync(context)` to continue.

## Unit of Work Rules
- `BeginAsync` starts a transaction.
- `CommitAsync` saves changes and commits.
- `RollbackAsync` rolls back.
- Repositories **never** call `SaveChangesAsync` directly.

## Controller Rules
- Controllers are **thin dispatchers**.
- No business logic.
- Map HTTP → command/query → handler → HTTP.
- Extract `AccountId` from JWT claim.

---

## Project placement

| Artifact | Project |
|---|---|
| Command / Query record | `FinClaude.Application/Features/{Feature}/Commands/{Op}/` |
| Context object | `FinClaude.Application/Features/{Feature}/Commands/{Op}/` |
| Handler | `FinClaude.Application/Features/{Feature}/Commands/{Op}/` |
| Query + Query Handler | `FinClaude.Application/Features/{Feature}/Queries/{Op}/` |
| DTOs | `FinClaude.Application/Features/{Feature}/DTOs/` |
| Steps | `FinClaude.Infrastructure/Features/{Feature}/Commands/Steps/` |
| ChainProvider | `FinClaude.Infrastructure/Features/{Feature}/Commands/` |
| Controller | `FinClaude.Api/Controllers/` |

---

## Code examples

### Command record (Application)
```csharp
public record CreateGoalCommand(
    Guid AccountId,
    string Name,
    decimal TargetValue,
    DateOnly TargetDate,
    Guid? AssetId,
    Guid? GroupId) : ICommand<GoalResponse>;
```

### Context object (Application)
```csharp
public class CreateGoalContext
{
    public required Guid AccountId { get; init; }
    public required string Name { get; init; }
    public required decimal TargetValue { get; init; }
    public required DateOnly TargetDate { get; init; }
    public Guid? AssetId { get; init; }
    public Guid? GroupId { get; init; }
    public Goal? CreatedGoal { get; set; }   // populated by step
}
```

### Handler (Application)
```csharp
public class CreateGoalCommandHandler(
    IChainProvider<CreateGoalContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<CreateGoalCommand, GoalResponse>
{
    public async Task<ErrorOr<GoalResponse>> HandleAsync(CreateGoalCommand command, CancellationToken ct = default)
    {
        var context = new CreateGoalContext { AccountId = command.AccountId, Name = command.Name, ... };
        var chain = chainProvider.GetChain();

        await uow.BeginAsync(ct);
        var result = await chain.ExecuteAsync(context, ct);

        if (result.IsError) { await uow.RollbackAsync(ct); return result.Errors; }

        await uow.CommitAsync(ct);
        var g = context.CreatedGoal!;
        return new GoalResponse(g.Id, g.Name, ...);
    }
}
```

### Step (Infrastructure)
```csharp
public class ValidateCreateGoalStep : BaseStep<CreateGoalContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateGoalContext context, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(context.Name))
            return Error.Validation("Goal.NameRequired", "Name is required.");

        return await NextAsync(context, ct);
    }
}

public class PersistCreateGoalStep(AppDbContext db) : BaseStep<CreateGoalContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateGoalContext context, CancellationToken ct = default)
    {
        var goal = new Goal { AccountId = context.AccountId, Name = context.Name, ... };
        db.Goals.Add(goal);
        context.CreatedGoal = goal;
        return await NextAsync(context, ct);
    }
}
```

### ChainProvider (Infrastructure)
```csharp
public class CreateGoalChainProvider(
    ValidateCreateGoalStep validate,
    AuthorizeLinkedEntityForCreateStep authorizeLinked,
    PersistCreateGoalStep persist) : IChainProvider<CreateGoalContext>
{
    public IStep<CreateGoalContext> GetChain()
    {
        validate.SetNext(authorizeLinked).SetNext(persist);
        return validate;
    }
}
```

### Controller (Api)
```csharp
[ApiController]
[Route("api/v1/goals")]
[Authorize]
public class GoalsController(
    ICommandHandler<CreateGoalCommand, GoalResponse> createHandler,
    ICommandHandler<DeleteGoalCommand> deleteHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();
        var result = await createHandler.HandleAsync(new CreateGoalCommand(accountId.Value, request.Name, ...), ct);
        return result.ToActionResult(created => CreatedAtAction(nameof(GetGoal), new { id = created.Id }, created));
    }

    private Guid? GetAccountId()
    {
        var claim = User.FindFirstValue("account_id");
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
```
