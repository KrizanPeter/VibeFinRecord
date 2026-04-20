using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Goals.DTOs;

namespace FinClaude.Application.Features.Goals.Commands.UpdateGoal;

public class UpdateGoalCommandHandler(
    IChainProvider<UpdateGoalContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<UpdateGoalCommand, GoalResponse>
{
    public async Task<ErrorOr<GoalResponse>> HandleAsync(UpdateGoalCommand command, CancellationToken ct = default)
    {
        var context = new UpdateGoalContext
        {
            AccountId = command.AccountId,
            GoalId = command.GoalId,
            Name = command.Name,
            TargetValue = command.TargetValue,
            TargetDate = command.TargetDate,
            AssetId = command.AssetId,
            GroupId = command.GroupId,
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
        var g = context.Goal!;
        return new GoalResponse(g.Id, g.Name, g.TargetValue, g.TargetDate, g.AssetId, g.GroupId, null, null, g.CreatedAt);
    }
}
