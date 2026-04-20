using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Account.Commands.UpdateAccount;

namespace FinClaude.Infrastructure.Features.Account.Commands.Steps;

public class PersistAccountSetupStep : BaseStep<UpdateAccountContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateAccountContext context, CancellationToken ct = default)
    {
        var account = context.Account!;
        account.Currency = context.Currency;
        account.SnapshotStartDate = context.SnapshotStartDate;
        account.SnapshotPeriodicity = context.SnapshotPeriodicity;

        return await NextAsync(context, ct);
    }
}
