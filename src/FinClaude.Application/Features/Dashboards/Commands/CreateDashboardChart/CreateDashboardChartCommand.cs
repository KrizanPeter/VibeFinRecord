using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Dashboards.DTOs;
using FinClaude.Domain.Enums;

namespace FinClaude.Application.Features.Dashboards.Commands.CreateDashboardChart;

public record CreateDashboardChartCommand(
    Guid AccountId,
    string Name,
    ChartType ChartType,
    SourceType SourceType,
    Guid? AssetId,
    Guid? GroupId) : ICommand<DashboardChartResponse>;
