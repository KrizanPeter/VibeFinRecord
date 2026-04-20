using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Goals.DTOs;
using FinClaude.Application.Features.Goals.Queries.ListGoals;
using FinClaude.Domain.Entities;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Goals.Queries;

public class ListGoalsQueryHandler(AppDbContext db) : IQueryHandler<ListGoalsQuery, List<GoalResponse>>
{
    public async Task<ErrorOr<List<GoalResponse>>> HandleAsync(ListGoalsQuery query, CancellationToken ct = default)
    {
        var goals = await db.Goals
            .AsNoTracking()
            .Where(g => g.AccountId == query.AccountId && g.DeletedAt == null)
            .OrderBy(g => g.Name)
            .ToListAsync(ct);

        var latestSnapshot = await db.Snapshots
            .AsNoTracking()
            .Where(s => s.AccountId == query.AccountId && s.DeletedAt == null)
            .OrderByDescending(s => s.SnapshotDate)
            .FirstOrDefaultAsync(ct);

        var responses = new List<GoalResponse>(goals.Count);
        foreach (var goal in goals)
        {
            var (currentValue, progressPercent) = await CalculateProgressAsync(goal, latestSnapshot, ct);
            responses.Add(new GoalResponse(
                goal.Id, goal.Name, goal.TargetValue, goal.TargetDate,
                goal.AssetId, goal.GroupId,
                currentValue, progressPercent, goal.CreatedAt));
        }

        return responses;
    }

    private async Task<(decimal? CurrentValue, decimal? ProgressPercent)> CalculateProgressAsync(
        Goal goal, Domain.Entities.Snapshot? latestSnapshot, CancellationToken ct)
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
