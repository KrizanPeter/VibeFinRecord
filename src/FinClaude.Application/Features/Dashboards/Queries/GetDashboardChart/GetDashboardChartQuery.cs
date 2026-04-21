using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Dashboards.DTOs;

namespace FinClaude.Application.Features.Dashboards.Queries.GetDashboardChart;

public record GetDashboardChartQuery(Guid AccountId, Guid ChartId) : IQuery<DashboardChartResponse>;
