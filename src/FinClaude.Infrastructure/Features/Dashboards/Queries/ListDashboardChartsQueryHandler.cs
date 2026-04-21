using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Dashboards.DTOs;
using FinClaude.Application.Features.Dashboards.Queries.ListDashboardCharts;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Dashboards.Queries;

public class ListDashboardChartsQueryHandler(AppDbContext db) : IQueryHandler<ListDashboardChartsQuery, List<DashboardChartResponse>>
{
    public async Task<ErrorOr<List<DashboardChartResponse>>> HandleAsync(ListDashboardChartsQuery query, CancellationToken ct = default)
    {
        var charts = await db.DashboardCharts
            .AsNoTracking()
            .Where(c => c.AccountId == query.AccountId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new DashboardChartResponse(c.Id, c.Name, c.ChartType, c.SourceType, c.AssetId, c.GroupId, c.CreatedAt))
            .ToListAsync(ct);

        return charts;
    }
}
