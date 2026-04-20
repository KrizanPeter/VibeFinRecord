using FinClaude.Domain.Common;
using FinClaude.Domain.Enums;

namespace FinClaude.Domain.Entities;

public class Account : BaseEntity
{
    public string IdentityUserId { get; set; } = string.Empty;
    public string? Currency { get; set; }
    public DateOnly? SnapshotStartDate { get; set; }
    public SnapshotPeriodicity? SnapshotPeriodicity { get; set; }
}
