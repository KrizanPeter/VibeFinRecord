using FinClaude.Domain.Common;

namespace FinClaude.Domain.Entities;

public class AssetGroup : BaseEntity
{
    public Guid AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
}
