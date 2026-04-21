using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Dashboards.DTOs;

namespace FinClaude.Application.Features.Dashboards.Queries.ListDashboardCharts;

public record ListDashboardChartsQuery(Guid AccountId) : IQuery<List<DashboardChartResponse>>;
