using FinClaude.Domain.Common;
using FinClaude.Domain.Enums;

namespace FinClaude.Domain.Entities;

public class DashboardChart : BaseEntity
{
    public Guid AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ChartType ChartType { get; set; }
    public SourceType SourceType { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? GroupId { get; set; }
}
