using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Goals.DTOs;

namespace FinClaude.Application.Features.Goals.Queries.GetGoal;

public record GetGoalQuery(Guid AccountId, Guid GoalId) : IQuery<GoalResponse>;
