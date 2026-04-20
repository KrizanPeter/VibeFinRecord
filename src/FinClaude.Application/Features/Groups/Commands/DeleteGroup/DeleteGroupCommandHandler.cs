using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;

namespace FinClaude.Application.Features.Groups.Commands.DeleteGroup;

public class DeleteGroupCommandHandler(
    IChainProvider<DeleteGroupContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<DeleteGroupCommand>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteGroupCommand command, CancellationToken ct = default)
    {
        var context = new DeleteGroupContext { AccountId = command.AccountId, GroupId = command.GroupId };
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
