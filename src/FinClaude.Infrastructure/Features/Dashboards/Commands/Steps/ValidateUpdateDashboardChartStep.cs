using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Dashboards.Commands.UpdateDashboardChart;
using FinClaude.Domain.Enums;

namespace FinClaude.Infrastructure.Features.Dashboards.Commands.Steps;

public class ValidateUpdateDashboardChartStep : BaseStep<UpdateDashboardChartContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateDashboardChartContext context, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(context.Name))
            return Error.Validation("Chart.NameRequired", "Chart name is required.");

        if (context.Name.Length > 200)
            return Error.Validation("Chart.NameTooLong", "Chart name cannot exceed 200 characters.");

        if (context.AssetId.HasValue == context.GroupId.HasValue)
            return Error.Failure("Chart.LinkedEntityInvalid", "A chart must link to exactly one Asset or AssetGroup.");

        if (context.ChartType == ChartType.Pie && context.SourceType == SourceType.Asset)
            return Error.Failure("Chart.PieAssetInvalid", "Pie charts are only supported for AssetGroup source type.");

        return await NextAsync(context, ct);
    }
}
