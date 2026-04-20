using FinClaude.Domain.Common;

namespace FinClaude.Domain.Entities;

public class Goal : BaseEntity
{
    public Guid AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetValue { get; set; }
    public DateOnly TargetDate { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? GroupId { get; set; }
}
