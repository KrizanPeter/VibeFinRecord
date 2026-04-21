using FinClaude.Domain.Entities;
using FinClaude.Domain.Enums;

namespace FinClaude.Application.Features.Dashboards.Commands.UpdateDashboardChart;

public class UpdateDashboardChartContext
{
    public required Guid AccountId { get; init; }
    public required Guid ChartId { get; init; }
    public required string Name { get; init; }
    public required ChartType ChartType { get; init; }
    public required SourceType SourceType { get; init; }
    public Guid? AssetId { get; init; }
    public Guid? GroupId { get; init; }
    public DashboardChart? Chart { get; set; }
}
