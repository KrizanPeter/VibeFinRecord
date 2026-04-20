using FinClaude.Domain.Entities;

namespace FinClaude.Application.Features.Groups.Commands.UpdateGroup;

public class UpdateGroupContext
{
    public required Guid AccountId { get; init; }
    public required Guid GroupId { get; init; }
    public required string Name { get; init; }
    public AssetGroup? Group { get; set; }
}
