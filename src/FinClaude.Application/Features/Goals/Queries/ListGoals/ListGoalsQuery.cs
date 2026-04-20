using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Goals.DTOs;

namespace FinClaude.Application.Features.Goals.Queries.ListGoals;

public record ListGoalsQuery(Guid AccountId) : IQuery<List<GoalResponse>>;
