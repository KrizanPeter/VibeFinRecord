namespace FinClaude.Application.Features.Snapshots.DTOs;

public record AssetSnapshotEntry(
    Guid AssetId,
    string AssetName,
    decimal Value);
