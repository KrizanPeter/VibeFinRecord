using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Goals.Commands.DeleteGoal;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Goals.Commands.Steps;

public class AuthorizeAndDeleteGoalStep(AppDbContext db) : BaseStep<DeleteGoalContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(DeleteGoalContext context, CancellationToken ct = default)
    {
        var goal = await db.Goals.FirstOrDefaultAsync(
            g => g.Id == context.GoalId && g.AccountId == context.AccountId && g.DeletedAt == null, ct);

        if (goal is null)
            return Error.NotFound("Goal.NotFound", "Goal not found.");

        goal.DeletedAt = DateTime.UtcNow;

        return await NextAsync(context, ct);
    }
}
