using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Account.DTOs;

namespace FinClaude.Application.Features.Account.Commands.UpdateAccount;

public class UpdateAccountCommandHandler(
    IChainProvider<UpdateAccountContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<UpdateAccountCommand, AccountResponse>
{
    public async Task<ErrorOr<AccountResponse>> HandleAsync(UpdateAccountCommand command, CancellationToken ct = default)
    {
        var context = new UpdateAccountContext
        {
            AccountId = command.AccountId,
            Currency = command.Currency,
            SnapshotStartDate = command.SnapshotStartDate,
            SnapshotPeriodicity = command.SnapshotPeriodicity,
        };

        var chain = chainProvider.GetChain();

        await uow.BeginAsync(ct);
        var result = await chain.ExecuteAsync(context, ct);

        if (result.IsError)
        {
            await uow.RollbackAsync(ct);
            return result.Errors;
        }

        await uow.CommitAsync(ct);

        var account = context.Account!;
        return new AccountResponse(
            account.Id,
            account.Currency,
            account.SnapshotStartDate,
            account.SnapshotPeriodicity,
            account.SnapshotStartDate.HasValue);
    }
}
