using FinClaude.Domain.Entities;

namespace FinClaude.Application.Features.Assets.Commands.UpdateAsset;

public class UpdateAssetContext
{
    public required Guid AccountId { get; init; }
    public required Guid AssetId { get; init; }
    public required string Name { get; init; }
    public string? Institution { get; init; }
    public Asset? Asset { get; set; }
}
