using FinClaude.Domain.Entities;
using FinClaude.Domain.Enums;

namespace FinClaude.Application.Features.Dashboards.Commands.CreateDashboardChart;

public class CreateDashboardChartContext
{
    public required Guid AccountId { get; init; }
    public required string Name { get; init; }
    public required ChartType ChartType { get; init; }
    public required SourceType SourceType { get; init; }
    public Guid? AssetId { get; init; }
    public Guid? GroupId { get; init; }
    public DashboardChart? CreatedChart { get; set; }
}
