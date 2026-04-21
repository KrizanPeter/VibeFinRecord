using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Dashboards.DTOs;

namespace FinClaude.Application.Features.Dashboards.Queries.GetChartData;

public record GetChartDataQuery(Guid AccountId, Guid ChartId, string Range) : IQuery<ChartDataResponse>;
