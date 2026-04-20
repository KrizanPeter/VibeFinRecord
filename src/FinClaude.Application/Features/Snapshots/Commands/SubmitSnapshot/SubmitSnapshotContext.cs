using FinClaude.Domain.Entities;

namespace FinClaude.Application.Features.Snapshots.Commands.SubmitSnapshot;

public class SubmitSnapshotContext
{
    public required Guid AccountId { get; init; }
    public required DateOnly SnapshotDate { get; init; }
    public required List<AssetValueInput> Assets { get; init; }
    public List<Asset> ActiveAssets { get; set; } = [];
    public Snapshot? CreatedSnapshot { get; set; }
}
