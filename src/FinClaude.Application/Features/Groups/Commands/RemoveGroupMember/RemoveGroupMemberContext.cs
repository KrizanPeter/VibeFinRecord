namespace FinClaude.Application.Features.Groups.Commands.RemoveGroupMember;

public class RemoveGroupMemberContext
{
    public required Guid AccountId { get; init; }
    public required Guid GroupId { get; init; }
    public required Guid AssetId { get; init; }
}
