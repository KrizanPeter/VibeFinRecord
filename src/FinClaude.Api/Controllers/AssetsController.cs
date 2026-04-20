using System.Security.Claims;
using FinClaude.Api.Common.Errors;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Assets.Commands.CreateAsset;
using FinClaude.Application.Features.Assets.Commands.DeleteAsset;
using FinClaude.Application.Features.Assets.Commands.UpdateAsset;
using FinClaude.Application.Features.Assets.DTOs;
using FinClaude.Application.Features.Assets.Queries.GetAsset;
using FinClaude.Application.Features.Assets.Queries.ListAssets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinClaude.Api.Controllers;

[ApiController]
[Route("api/v1/assets")]
[Authorize]
public class AssetsController(
    IQueryHandler<ListAssetsQuery, List<AssetResponse>> listHandler,
    IQueryHandler<GetAssetQuery, AssetResponse> getHandler,
    ICommandHandler<CreateAssetCommand, AssetResponse> createHandler,
    ICommandHandler<UpdateAssetCommand, AssetResponse> updateHandler,
    ICommandHandler<DeleteAssetCommand> deleteHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListAssets(CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await listHandler.HandleAsync(new ListAssetsQuery(accountId.Value), ct);
        return result.ToActionResult(Ok);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsset(Guid id, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await getHandler.HandleAsync(new GetAssetQuery(accountId.Value, id), ct);
        return result.ToActionResult(Ok);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsset([FromBody] CreateAssetRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await createHandler.HandleAsync(new CreateAssetCommand(accountId.Value, request.Name, request.Institution), ct);
        return result.ToActionResult(created => CreatedAtAction(nameof(GetAsset), new { id = created.Id }, created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsset(Guid id, [FromBody] UpdateAssetRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await updateHandler.HandleAsync(new UpdateAssetCommand(accountId.Value, id, request.Name, request.Institution), ct);
        return result.ToActionResult(Ok);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsset(Guid id, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await deleteHandler.HandleAsync(new DeleteAssetCommand(accountId.Value, id), ct);
        return result.ToActionResult(_ => NoContent());
    }

    private Guid? GetAccountId()
    {
        var claim = User.FindFirstValue("account_id");
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}

public record CreateAssetRequest(string Name, string? Institution);
public record UpdateAssetRequest(string Name, string? Institution);
