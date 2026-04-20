using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Account.DTOs;
using FinClaude.Application.Features.Account.Queries.GetAccount;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Account.Queries;

public class GetAccountQueryHandler(AppDbContext db) : IQueryHandler<GetAccountQuery, AccountResponse>
{
    public async Task<ErrorOr<AccountResponse>> HandleAsync(GetAccountQuery query, CancellationToken ct = default)
    {
        var account = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == query.AccountId, ct);

        if (account is null)
            return Error.NotFound("Account.NotFound", "Account not found.");

        return new AccountResponse(
            account.Id,
            account.Currency,
            account.SnapshotStartDate,
            account.SnapshotPeriodicity,
            account.SnapshotStartDate.HasValue);
    }
}
