namespace FinClaude.Application.Features.Goals.DTOs;

public record GoalResponse(
    Guid Id,
    string Name,
    decimal TargetValue,
    DateOnly TargetDate,
    Guid? AssetId,
    Guid? GroupId,
    decimal? CurrentValue,
    decimal? ProgressPercent,
    DateTime CreatedAt);
