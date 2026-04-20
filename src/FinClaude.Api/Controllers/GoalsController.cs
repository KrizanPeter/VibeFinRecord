using System.Security.Claims;
using FinClaude.Api.Common.Errors;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Goals.Commands.CreateGoal;
using FinClaude.Application.Features.Goals.Commands.DeleteGoal;
using FinClaude.Application.Features.Goals.Commands.UpdateGoal;
using FinClaude.Application.Features.Goals.DTOs;
using FinClaude.Application.Features.Goals.Queries.GetGoal;
using FinClaude.Application.Features.Goals.Queries.ListGoals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinClaude.Api.Controllers;

[ApiController]
[Route("api/v1/goals")]
[Authorize]
public class GoalsController(
    IQueryHandler<ListGoalsQuery, List<GoalResponse>> listHandler,
    IQueryHandler<GetGoalQuery, GoalResponse> getHandler,
    ICommandHandler<CreateGoalCommand, GoalResponse> createHandler,
    ICommandHandler<UpdateGoalCommand, GoalResponse> updateHandler,
    ICommandHandler<DeleteGoalCommand> deleteHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListGoals(CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await listHandler.HandleAsync(new ListGoalsQuery(accountId.Value), ct);
        return result.ToActionResult(Ok);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGoal(Guid id, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await getHandler.HandleAsync(new GetGoalQuery(accountId.Value, id), ct);
        return result.ToActionResult(Ok);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await createHandler.HandleAsync(
            new CreateGoalCommand(accountId.Value, request.Name, request.TargetValue, request.TargetDate, request.AssetId, request.GroupId), ct);
        return result.ToActionResult(created => CreatedAtAction(nameof(GetGoal), new { id = created.Id }, created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGoal(Guid id, [FromBody] UpdateGoalRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await updateHandler.HandleAsync(
            new UpdateGoalCommand(accountId.Value, id, request.Name, request.TargetValue, request.TargetDate, request.AssetId, request.GroupId), ct);
        return result.ToActionResult(Ok);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGoal(Guid id, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await deleteHandler.HandleAsync(new DeleteGoalCommand(accountId.Value, id), ct);
        return result.ToActionResult(_ => NoContent());
    }

    private Guid? GetAccountId()
    {
        var claim = User.FindFirstValue("account_id");
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}

public record CreateGoalRequest(string Name, decimal TargetValue, DateOnly TargetDate, Guid? AssetId, Guid? GroupId);
public record UpdateGoalRequest(string Name, decimal TargetValue, DateOnly TargetDate, Guid? AssetId, Guid? GroupId);
