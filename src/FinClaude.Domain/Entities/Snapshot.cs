using FinClaude.Domain.Common;

namespace FinClaude.Domain.Entities;

public class Snapshot : BaseEntity
{
    public Guid AccountId { get; set; }
    public DateOnly SnapshotDate { get; set; }
}
