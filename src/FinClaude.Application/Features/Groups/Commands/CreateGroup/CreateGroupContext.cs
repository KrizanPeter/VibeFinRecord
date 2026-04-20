using FinClaude.Domain.Entities;

namespace FinClaude.Application.Features.Groups.Commands.CreateGroup;

public class CreateGroupContext
{
    public required Guid AccountId { get; init; }
    public required string Name { get; init; }
    public AssetGroup? CreatedGroup { get; set; }
}
