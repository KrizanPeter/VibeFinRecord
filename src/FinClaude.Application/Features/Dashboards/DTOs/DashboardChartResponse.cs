using FinClaude.Domain.Enums;

namespace FinClaude.Application.Features.Dashboards.DTOs;

public record DashboardChartResponse(
    Guid Id,
    string Name,
    ChartType ChartType,
    SourceType SourceType,
    Guid? AssetId,
    Guid? GroupId,
    DateTime CreatedAt);
