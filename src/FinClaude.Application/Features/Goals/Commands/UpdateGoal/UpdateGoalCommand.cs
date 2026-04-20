using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Goals.DTOs;

namespace FinClaude.Application.Features.Goals.Commands.UpdateGoal;

public record UpdateGoalCommand(
    Guid AccountId,
    Guid GoalId,
    string Name,
    decimal TargetValue,
    DateOnly TargetDate,
    Guid? AssetId,
    Guid? GroupId) : ICommand<GoalResponse>;
