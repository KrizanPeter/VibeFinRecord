using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Dashboards.DTOs;
using FinClaude.Application.Features.Dashboards.Queries.GetChartData;
using FinClaude.Domain.Enums;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Dashboards.Queries;

public class GetChartDataQueryHandler(AppDbContext db) : IQueryHandler<GetChartDataQuery, ChartDataResponse>
{
    public async Task<ErrorOr<ChartDataResponse>> HandleAsync(GetChartDataQuery query, CancellationToken ct = default)
    {
        var chart = await db.DashboardCharts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.ChartId && c.AccountId == query.AccountId, ct);

        if (chart is null)
            return Error.NotFound("Chart.NotFound", "Dashboard chart not found.");

        var cutoff = query.Range switch
        {
            "3m" => (DateOnly?)DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)),
            "6m" => DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)),
            "1y" => DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            _ => null,
        };

        List<ChartDataPoint> points;

        if (chart.ChartType is ChartType.Line or ChartType.Bar && chart.SourceType == SourceType.Asset)
        {
            points = await ComputeAssetTimeSeriesAsync(chart.AssetId!.Value, query.AccountId, cutoff, ct);
        }
        else if (chart.ChartType is ChartType.Line or ChartType.Bar && chart.SourceType == SourceType.AssetGroup)
        {
            points = await ComputeGroupTimeSeriesAsync(chart.GroupId!.Value, query.AccountId, cutoff, ct);
        }
        else
        {
            points = await ComputePieDataAsync(chart.GroupId!.Value, query.AccountId, ct);
        }

        return new ChartDataResponse(points);
    }

    private async Task<List<ChartDataPoint>> ComputeAssetTimeSeriesAsync(
        Guid assetId, Guid accountId, DateOnly? cutoff, CancellationToken ct)
    {
        return await db.Snapshots
            .AsNoTracking()
            .Where(s => s.AccountId == accountId && (cutoff == null || s.SnapshotDate >= cutoff))
            .Join(
                db.AssetSnapshots.Where(a => a.AssetId == assetId),
                s => s.Id,
                a => a.SnapshotId,
                (s, a) => new ChartDataPoint(s.SnapshotDate, null, a.Value, null))
            .OrderBy(p => p.Date)
            .ToListAsync(ct);
    }

    private async Task<List<ChartDataPoint>> ComputeGroupTimeSeriesAsync(
        Guid groupId, Guid accountId, DateOnly? cutoff, CancellationToken ct)
    {
        var memberIds = await db.AssetGroupMemberships
            .Where(m => m.GroupId == groupId)
            .Select(m => m.AssetId)
            .ToListAsync(ct);

        if (memberIds.Count == 0)
            return [];

        return await db.Snapshots
            .AsNoTracking()
            .Where(s => s.AccountId == accountId && (cutoff == null || s.SnapshotDate >= cutoff))
            .Join(
                db.AssetSnapshots.Where(a => memberIds.Contains(a.AssetId)),
                s => s.Id,
                a => a.SnapshotId,
                (s, a) => new { s.SnapshotDate, a.Value })
            .GroupBy(x => x.SnapshotDate)
            .Select(g => new ChartDataPoint(g.Key, null, g.Sum(x => x.Value), null))
            .OrderBy(p => p.Date)
            .ToListAsync(ct);
    }

    private async Task<List<ChartDataPoint>> ComputePieDataAsync(
        Guid groupId, Guid accountId, CancellationToken ct)
    {
        var latestSnapshot = await db.Snapshots
            .AsNoTracking()
            .Where(s => s.AccountId == accountId)
            .OrderByDescending(s => s.SnapshotDate)
            .FirstOrDefaultAsync(ct);

        if (latestSnapshot is null)
            return [];

        var memberIds = await db.AssetGroupMemberships
            .Where(m => m.GroupId == groupId)
            .Select(m => m.AssetId)
            .ToListAsync(ct);

        if (memberIds.Count == 0)
            return [];

        var entries = await db.AssetSnapshots
            .AsNoTracking()
            .Where(a => a.SnapshotId == latestSnapshot.Id && memberIds.Contains(a.AssetId))
            .Join(db.Assets, a => a.AssetId, asset => asset.Id,
                (a, asset) => new { asset.Name, a.Value })
            .ToListAsync(ct);

        if (entries.Count == 0)
            return [];

        var total = entries.Sum(e => e.Value);

        return entries
            .Select(e => new ChartDataPoint(
                null,
                e.Name,
                e.Value,
                total == 0 ? 0 : e.Value / total * 100))
            .ToList();
    }
}
