using FinClaude.Domain.Common;

namespace FinClaude.Domain.Entities;

public class AssetSnapshot : BaseEntity
{
    public Guid SnapshotId { get; set; }
    public Guid AssetId { get; set; }
    public decimal Value { get; set; }
}
