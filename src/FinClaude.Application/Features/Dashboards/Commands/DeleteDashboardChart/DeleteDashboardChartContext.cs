using FinClaude.Domain.Entities;

namespace FinClaude.Application.Features.Dashboards.Commands.DeleteDashboardChart;

public class DeleteDashboardChartContext
{
    public required Guid AccountId { get; init; }
    public required Guid ChartId { get; init; }
    public DashboardChart? Chart { get; set; }
}
