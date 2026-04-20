using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;

namespace FinClaude.Application.Features.Groups.Commands.RemoveGroupMember;

public class RemoveGroupMemberCommandHandler(
    IChainProvider<RemoveGroupMemberContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<RemoveGroupMemberCommand>
{
    public async Task<ErrorOr<Success>> HandleAsync(RemoveGroupMemberCommand command, CancellationToken ct = default)
    {
        var context = new RemoveGroupMemberContext { AccountId = command.AccountId, GroupId = command.GroupId, AssetId = command.AssetId };
        var chain = chainProvider.GetChain();

        await uow.BeginAsync(ct);
        var result = await chain.ExecuteAsync(context, ct);

        if (result.IsError)
        {
            await uow.RollbackAsync(ct);
            return result.Errors;
        }

        await uow.CommitAsync(ct);
        return Result.Success;
    }
}
