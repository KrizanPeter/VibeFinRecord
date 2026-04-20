namespace FinClaude.Application.Features.Snapshots.DTOs;

public record SnapshotSummaryResponse(
    Guid Id,
    DateOnly SnapshotDate,
    decimal TotalNetWorth,
    DateTime CreatedAt);
