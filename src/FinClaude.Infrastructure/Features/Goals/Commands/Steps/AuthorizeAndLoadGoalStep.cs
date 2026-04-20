using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Goals.Commands.UpdateGoal;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Goals.Commands.Steps;

public class AuthorizeAndLoadGoalStep(AppDbContext db) : BaseStep<UpdateGoalContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateGoalContext context, CancellationToken ct = default)
    {
        var goal = await db.Goals.FirstOrDefaultAsync(
            g => g.Id == context.GoalId && g.AccountId == context.AccountId && g.DeletedAt == null, ct);

        if (goal is null)
            return Error.NotFound("Goal.NotFound", "Goal not found.");

        context.Goal = goal;
        return await NextAsync(context, ct);
    }
}
