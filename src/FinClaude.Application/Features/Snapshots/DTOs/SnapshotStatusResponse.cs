namespace FinClaude.Application.Features.Snapshots.DTOs;

public record SnapshotStatusResponse(
    List<DateOnly> MissingDates,
    DateOnly? NextExpectedDate);
