using FinClaude.Application.Common.CQRS;

namespace FinClaude.Application.Features.Dashboards.Commands.DeleteDashboardChart;

public record DeleteDashboardChartCommand(Guid AccountId, Guid ChartId) : ICommand;
