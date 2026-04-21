using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Dashboards.Commands.UpdateDashboardChart;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Dashboards.Commands.Steps;

public class AuthorizeAndLoadDashboardChartStep(AppDbContext db) : BaseStep<UpdateDashboardChartContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateDashboardChartContext context, CancellationToken ct = default)
    {
        var chart = await db.DashboardCharts.FirstOrDefaultAsync(
            c => c.Id == context.ChartId && c.AccountId == context.AccountId, ct);

        if (chart is null)
            return Error.NotFound("Chart.NotFound", "Dashboard chart not found.");

        context.Chart = chart;
        return await NextAsync(context, ct);
    }
}
