using FinClaude.Domain.Entities;

namespace FinClaude.Application.Features.Assets.Commands.DeleteAsset;

public class DeleteAssetContext
{
    public required Guid AccountId { get; init; }
    public required Guid AssetId { get; init; }
    public Asset? Asset { get; set; }
}
