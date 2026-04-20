using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Goals.DTOs;

namespace FinClaude.Application.Features.Goals.Commands.CreateGoal;

public record CreateGoalCommand(
    Guid AccountId,
    string Name,
    decimal TargetValue,
    DateOnly TargetDate,
    Guid? AssetId,
    Guid? GroupId) : ICommand<GoalResponse>;
