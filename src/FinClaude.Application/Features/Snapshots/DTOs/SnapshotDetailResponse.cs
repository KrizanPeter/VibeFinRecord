namespace FinClaude.Application.Features.Snapshots.DTOs;

public record SnapshotDetailResponse(
    Guid Id,
    DateOnly SnapshotDate,
    List<AssetSnapshotEntry> Assets,
    DateTime CreatedAt);
