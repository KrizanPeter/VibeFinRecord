using System.Security.Claims;
using FinClaude.Api.Common.Errors;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.DTOs;
using FinClaude.Application.Features.Snapshots.Commands.SubmitSnapshot;
using FinClaude.Application.Features.Snapshots.DTOs;
using FinClaude.Application.Features.Snapshots.Queries.GetSnapshot;
using FinClaude.Application.Features.Snapshots.Queries.GetSnapshotStatus;
using FinClaude.Application.Features.Snapshots.Queries.ListSnapshots;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinClaude.Api.Controllers;

[ApiController]
[Route("api/v1/snapshots")]
[Authorize]
public class SnapshotsController(
    IQueryHandler<GetSnapshotStatusQuery, SnapshotStatusResponse> statusHandler,
    ICommandHandler<SubmitSnapshotCommand, SnapshotDetailResponse> submitHandler,
    IQueryHandler<ListSnapshotsQuery, PagedResponse<SnapshotSummaryResponse>> listHandler,
    IQueryHandler<GetSnapshotQuery, SnapshotDetailResponse> getHandler) : ControllerBase
{
    [HttpGet("status")]
    public async Task<IActionResult> GetSnapshotStatus(CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await statusHandler.HandleAsync(new GetSnapshotStatusQuery(accountId.Value), ct);
        return result.ToActionResult(Ok);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitSnapshot([FromBody] SubmitSnapshotRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var assets = request.Assets.Select(a => new AssetValueInput(a.AssetId, a.Value)).ToList();
        var command = new SubmitSnapshotCommand(accountId.Value, request.SnapshotDate, assets);
        var result = await submitHandler.HandleAsync(command, ct);
        return result.ToActionResult(r => CreatedAtAction(nameof(GetSnapshot), new { id = r.Id }, r));
    }

    [HttpGet]
    public async Task<IActionResult> ListSnapshots([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await listHandler.HandleAsync(new ListSnapshotsQuery(accountId.Value, page, pageSize), ct);
        return result.ToActionResult(Ok);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSnapshot(Guid id, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await getHandler.HandleAsync(new GetSnapshotQuery(accountId.Value, id), ct);
        return result.ToActionResult(Ok);
    }

    private Guid? GetAccountId()
    {
        var claim = User.FindFirstValue("account_id");
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}

public record SubmitSnapshotRequest(DateOnly SnapshotDate, List<AssetValueRequest> Assets);
public record AssetValueRequest(Guid AssetId, decimal Value);
