using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;

namespace FinClaude.Application.Features.Groups.Commands.AddGroupMember;

public class AddGroupMemberCommandHandler(
    IChainProvider<AddGroupMemberContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<AddGroupMemberCommand>
{
    public async Task<ErrorOr<Success>> HandleAsync(AddGroupMemberCommand command, CancellationToken ct = default)
    {
        var context = new AddGroupMemberContext { AccountId = command.AccountId, GroupId = command.GroupId, AssetId = command.AssetId };
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
