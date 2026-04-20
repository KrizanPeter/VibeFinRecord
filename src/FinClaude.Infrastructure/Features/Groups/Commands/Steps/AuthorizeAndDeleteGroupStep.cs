using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.DeleteGroup;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Groups.Commands.Steps;

public class AuthorizeAndDeleteGroupStep(AppDbContext db) : BaseStep<DeleteGroupContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(DeleteGroupContext context, CancellationToken ct = default)
    {
        var group = await db.AssetGroups.FirstOrDefaultAsync(
            g => g.Id == context.GroupId && g.AccountId == context.AccountId, ct);

        if (group is null)
            return Error.NotFound("Group.NotFound", "Asset group not found.");

        var now = DateTime.UtcNow;
        group.DeletedAt = now;

        var goals = await db.Goals.Where(g => g.GroupId == context.GroupId).ToListAsync(ct);
        foreach (var goal in goals)
            goal.DeletedAt = now;

        var charts = await db.DashboardCharts.Where(c => c.GroupId == context.GroupId).ToListAsync(ct);
        foreach (var chart in charts)
            chart.DeletedAt = now;

        return await NextAsync(context, ct);
    }
}
