using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;

namespace FinClaude.Application.Features.Goals.Commands.DeleteGoal;

public class DeleteGoalCommandHandler(
    IChainProvider<DeleteGoalContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<DeleteGoalCommand>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteGoalCommand command, CancellationToken ct = default)
    {
        var context = new DeleteGoalContext { AccountId = command.AccountId, GoalId = command.GoalId };
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
