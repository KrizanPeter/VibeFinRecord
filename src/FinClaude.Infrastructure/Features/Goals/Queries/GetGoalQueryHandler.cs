using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Goals.DTOs;
using FinClaude.Application.Features.Goals.Queries.GetGoal;
using FinClaude.Domain.Entities;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Goals.Queries;

public class GetGoalQueryHandler(AppDbContext db) : IQueryHandler<GetGoalQuery, GoalResponse>
{
    public async Task<ErrorOr<GoalResponse>> HandleAsync(GetGoalQuery query, CancellationToken ct = default)
    {
        var goal = await db.Goals
            .AsNoTracking()
            .FirstOrDefaultAsync(
                g => g.Id == query.GoalId && g.AccountId == query.AccountId && g.DeletedAt == null, ct);

        if (goal is null)
            return Error.NotFound("Goal.NotFound", "Goal not found.");

        var latestSnapshot = await db.Snapshots
            .AsNoTracking()
            .Where(s => s.AccountId == query.AccountId && s.DeletedAt == null)
            .OrderByDescending(s => s.SnapshotDate)
            .FirstOrDefaultAsync(ct);

        var (currentValue, progressPercent) = await CalculateProgressAsync(goal, latestSnapshot, ct);

        return new GoalResponse(
            goal.Id, goal.Name, goal.TargetValue, goal.TargetDate,
            goal.AssetId, goal.GroupId,
            currentValue, progressPercent, goal.CreatedAt);
    }

    private async Task<(decimal? CurrentValue, decimal? ProgressPercent)> CalculateProgressAsync(
        Goal goal, Snapshot? latestSnapshot, CancellationToken ct)
    {
        if (latestSnapshot is null)
            return (null, null);

        decimal? currentValue;

        if (goal.AssetId.HasValue)
        {
            var assetSnapshot = await db.AssetSnapshots
                .AsNoTracking()
                .Where(a => a.SnapshotId == latestSnapshot.Id && a.AssetId == goal.AssetId)
                .FirstOrDefaultAsync(ct);
            currentValue = assetSnapshot?.Value;
        }
        else
        {
            var groupAssetIds = await db.AssetGroupMemberships
                .AsNoTracking()
                .Where(m => m.GroupId == goal.GroupId)
                .Select(m => m.AssetId)
                .ToListAsync(ct);

            currentValue = await db.AssetSnapshots
                .AsNoTracking()
                .Where(a => a.SnapshotId == latestSnapshot.Id && groupAssetIds.Contains(a.AssetId))
                .SumAsync(a => (decimal?)a.Value, ct);
        }

        var progressPercent = currentValue.HasValue && goal.TargetValue != 0
            ? currentValue.Value / goal.TargetValue * 100
            : (decimal?)null;

        return (currentValue, progressPercent);
    }
}
