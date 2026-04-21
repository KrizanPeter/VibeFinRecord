using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Dashboards.Commands.UpdateDashboardChart;

namespace FinClaude.Infrastructure.Features.Dashboards.Commands.Steps;

public class PersistUpdateDashboardChartStep : BaseStep<UpdateDashboardChartContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateDashboardChartContext context, CancellationToken ct = default)
    {
        var chart = context.Chart!;
        chart.Name = context.Name;
        chart.ChartType = context.ChartType;
        chart.SourceType = context.SourceType;
        chart.AssetId = context.AssetId;
        chart.GroupId = context.GroupId;

        return await NextAsync(context, ct);
    }
}
