using System.Security.Claims;
using FinClaude.Api.Common.Errors;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Groups.Commands.AddGroupMember;
using FinClaude.Application.Features.Groups.Commands.CreateGroup;
using FinClaude.Application.Features.Groups.Commands.DeleteGroup;
using FinClaude.Application.Features.Groups.Commands.RemoveGroupMember;
using FinClaude.Application.Features.Groups.Commands.UpdateGroup;
using FinClaude.Application.Features.Groups.DTOs;
using FinClaude.Application.Features.Groups.Queries.GetGroup;
using FinClaude.Application.Features.Groups.Queries.ListGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinClaude.Api.Controllers;

[ApiController]
[Route("api/v1/groups")]
[Authorize]
public class GroupsController(
    IQueryHandler<ListGroupsQuery, List<AssetGroupResponse>> listHandler,
    IQueryHandler<GetGroupQuery, AssetGroupDetailResponse> getHandler,
    ICommandHandler<CreateGroupCommand, AssetGroupResponse> createHandler,
    ICommandHandler<UpdateGroupCommand, AssetGroupResponse> updateHandler,
    ICommandHandler<DeleteGroupCommand> deleteHandler,
    ICommandHandler<AddGroupMemberCommand> addMemberHandler,
    ICommandHandler<RemoveGroupMemberCommand> removeMemberHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListGroups(CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await listHandler.HandleAsync(new ListGroupsQuery(accountId.Value), ct);
        return result.ToActionResult(Ok);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGroup(Guid id, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await getHandler.HandleAsync(new GetGroupQuery(accountId.Value, id), ct);
        return result.ToActionResult(Ok);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await createHandler.HandleAsync(new CreateGroupCommand(accountId.Value, request.Name), ct);
        return result.ToActionResult(created => CreatedAtAction(nameof(GetGroup), new { id = created.Id }, created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] UpdateGroupRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await updateHandler.HandleAsync(new UpdateGroupCommand(accountId.Value, id, request.Name), ct);
        return result.ToActionResult(Ok);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGroup(Guid id, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await deleteHandler.HandleAsync(new DeleteGroupCommand(accountId.Value, id), ct);
        return result.ToActionResult(_ => NoContent());
    }

    [HttpPost("{id:guid}/assets")]
    public async Task<IActionResult> AddGroupMember(Guid id, [FromBody] GroupMemberRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await addMemberHandler.HandleAsync(new AddGroupMemberCommand(accountId.Value, id, request.AssetId), ct);
        return result.ToActionResult(_ => NoContent());
    }

    [HttpDelete("{id:guid}/assets/{assetId:guid}")]
    public async Task<IActionResult> RemoveGroupMember(Guid id, Guid assetId, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await removeMemberHandler.HandleAsync(new RemoveGroupMemberCommand(accountId.Value, id, assetId), ct);
        return result.ToActionResult(_ => NoContent());
    }

    private Guid? GetAccountId()
    {
        var claim = User.FindFirstValue("account_id");
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}

public record CreateGroupRequest(string Name);
public record UpdateGroupRequest(string Name);
public record GroupMemberRequest(Guid AssetId);
