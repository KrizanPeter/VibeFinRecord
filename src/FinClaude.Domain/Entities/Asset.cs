using FinClaude.Domain.Common;

namespace FinClaude.Domain.Entities;

public class Asset : BaseEntity
{
    public Guid AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Institution { get; set; }
}
