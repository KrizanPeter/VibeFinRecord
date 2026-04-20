using FinClaude.Domain.Entities;

namespace FinClaude.Application.Features.Goals.Commands.UpdateGoal;

public class UpdateGoalContext
{
    public required Guid AccountId { get; init; }
    public required Guid GoalId { get; init; }
    public required string Name { get; init; }
    public required decimal TargetValue { get; init; }
    public required DateOnly TargetDate { get; init; }
    public Guid? AssetId { get; init; }
    public Guid? GroupId { get; init; }
    public Goal? Goal { get; set; }
}
