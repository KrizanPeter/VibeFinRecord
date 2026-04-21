using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Dashboards.DTOs;
using FinClaude.Domain.Enums;

namespace FinClaude.Application.Features.Dashboards.Commands.UpdateDashboardChart;

public record UpdateDashboardChartCommand(
    Guid AccountId,
    Guid ChartId,
    string Name,
    ChartType ChartType,
    SourceType SourceType,
    Guid? AssetId,
    Guid? GroupId) : ICommand<DashboardChartResponse>;
