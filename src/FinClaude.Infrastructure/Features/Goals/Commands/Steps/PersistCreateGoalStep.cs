using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Goals.Commands.CreateGoal;
using FinClaude.Domain.Entities;
using FinClaude.Infrastructure.Persistence;

namespace FinClaude.Infrastructure.Features.Goals.Commands.Steps;

public class PersistCreateGoalStep(AppDbContext db) : BaseStep<CreateGoalContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateGoalContext context, CancellationToken ct = default)
    {
        var goal = new Goal
        {
            AccountId = context.AccountId,
            Name = context.Name,
            TargetValue = context.TargetValue,
            TargetDate = context.TargetDate,
            AssetId = context.AssetId,
            GroupId = context.GroupId,
        };

        db.Goals.Add(goal);
        context.CreatedGoal = goal;

        return await NextAsync(context, ct);
    }
}
