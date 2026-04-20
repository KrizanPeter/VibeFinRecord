using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Account.Commands.UpdateAccount;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Account.Commands.Steps;

public class LockIfSnapshotsExistStep(AppDbContext db) : BaseStep<UpdateAccountContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateAccountContext context, CancellationToken ct = default)
    {
        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == context.AccountId, ct);
        if (account is null)
            return Error.NotFound("Account.NotFound", "Account not found.");

        context.Account = account;

        var hasSnapshots = await db.Snapshots.AnyAsync(s => s.AccountId == context.AccountId, ct);
        if (hasSnapshots)
        {
            var periodicityChanged = account.SnapshotPeriodicity != context.SnapshotPeriodicity;
            var startDateChanged = account.SnapshotStartDate != context.SnapshotStartDate;

            if (periodicityChanged || startDateChanged)
                return Error.Conflict("Account.Locked", "Snapshot start date and periodicity cannot be changed once snapshots exist.");
        }

        return await NextAsync(context, ct);
    }
}
