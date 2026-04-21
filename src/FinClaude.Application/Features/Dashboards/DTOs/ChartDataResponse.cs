namespace FinClaude.Application.Features.Dashboards.DTOs;

public record ChartDataResponse(List<ChartDataPoint> DataPoints);

public record ChartDataPoint(
    DateOnly? Date,
    string? Label,
    decimal Value,
    decimal? Share);
