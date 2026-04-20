using FinClaude.Domain.Entities;

namespace FinClaude.Application.Features.Assets.Commands.CreateAsset;

public class CreateAssetContext
{
    public required Guid AccountId { get; init; }
    public required string Name { get; init; }
    public string? Institution { get; init; }
    public Asset? CreatedAsset { get; set; }
}
