using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Dashboards.DTOs;
using FinClaude.Application.Features.Dashboards.Queries.GetDashboardChart;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Dashboards.Queries;

public class GetDashboardChartQueryHandler(AppDbContext db) : IQueryHandler<GetDashboardChartQuery, DashboardChartResponse>
{
    public async Task<ErrorOr<DashboardChartResponse>> HandleAsync(GetDashboardChartQuery query, CancellationToken ct = default)
    {
        var chart = await db.DashboardCharts
            .AsNoTracking()
            .Where(c => c.Id == query.ChartId && c.AccountId == query.AccountId)
            .Select(c => new DashboardChartResponse(c.Id, c.Name, c.ChartType, c.SourceType, c.AssetId, c.GroupId, c.CreatedAt))
            .FirstOrDefaultAsync(ct);

        if (chart is null)
            return Error.NotFound("Chart.NotFound", "Dashboard chart not found.");

        return chart;
    }
}
