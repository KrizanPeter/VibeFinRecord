using FinClaude.Application.Common.CQRS;

namespace FinClaude.Application.Features.Goals.Commands.DeleteGoal;

public record DeleteGoalCommand(Guid AccountId, Guid GoalId) : ICommand;
