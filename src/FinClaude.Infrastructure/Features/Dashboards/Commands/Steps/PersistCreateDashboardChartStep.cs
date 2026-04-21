using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Dashboards.Commands.CreateDashboardChart;
using FinClaude.Domain.Entities;
using FinClaude.Infrastructure.Persistence;

namespace FinClaude.Infrastructure.Features.Dashboards.Commands.Steps;

public class PersistCreateDashboardChartStep(AppDbContext db) : BaseStep<CreateDashboardChartContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateDashboardChartContext context, CancellationToken ct = default)
    {
        var chart = new DashboardChart
        {
            AccountId = context.AccountId,
            Name = context.Name,
            ChartType = context.ChartType,
            SourceType = context.SourceType,
            AssetId = context.AssetId,
            GroupId = context.GroupId,
        };

        db.DashboardCharts.Add(chart);
        context.CreatedChart = chart;

        return await NextAsync(context, ct);
    }
}
