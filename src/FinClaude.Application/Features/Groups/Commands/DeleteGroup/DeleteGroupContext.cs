namespace FinClaude.Application.Features.Groups.Commands.DeleteGroup;

public class DeleteGroupContext
{
    public required Guid AccountId { get; init; }
    public required Guid GroupId { get; init; }
}
