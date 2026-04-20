namespace FinClaude.Application.Features.Goals.Commands.DeleteGoal;

public class DeleteGoalContext
{
    public required Guid AccountId { get; init; }
    public required Guid GoalId { get; init; }
}
