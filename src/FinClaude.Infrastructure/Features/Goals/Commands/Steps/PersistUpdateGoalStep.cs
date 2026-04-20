using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Goals.Commands.UpdateGoal;

namespace FinClaude.Infrastructure.Features.Goals.Commands.Steps;

public class PersistUpdateGoalStep : BaseStep<UpdateGoalContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateGoalContext context, CancellationToken ct = default)
    {
        var goal = context.Goal!;
        goal.Name = context.Name;
        goal.TargetValue = context.TargetValue;
        goal.TargetDate = context.TargetDate;
        goal.AssetId = context.AssetId;
        goal.GroupId = context.GroupId;

        return await NextAsync(context, ct);
    }
}
