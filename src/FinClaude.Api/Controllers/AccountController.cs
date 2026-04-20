using System.Security.Claims;
using FinClaude.Api.Common.Errors;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Account.Commands.UpdateAccount;
using FinClaude.Application.Features.Account.DTOs;
using FinClaude.Application.Features.Account.Queries.GetAccount;
using FinClaude.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinClaude.Api.Controllers;

[ApiController]
[Route("api/v1/account")]
[Authorize]
public class AccountController(
    IQueryHandler<GetAccountQuery, AccountResponse> getAccountHandler,
    ICommandHandler<UpdateAccountCommand, AccountResponse> updateAccountHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAccount(CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await getAccountHandler.HandleAsync(new GetAccountQuery(accountId.Value), ct);
        return result.ToActionResult(Ok);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var command = new UpdateAccountCommand(accountId.Value, request.Currency, request.SnapshotStartDate, request.SnapshotPeriodicity);
        var result = await updateAccountHandler.HandleAsync(command, ct);
        return result.ToActionResult(Ok);
    }

    private Guid? GetAccountId()
    {
        var claim = User.FindFirstValue("account_id");
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}

public record UpdateAccountRequest(string Currency, DateOnly SnapshotStartDate, SnapshotPeriodicity SnapshotPeriodicity);
