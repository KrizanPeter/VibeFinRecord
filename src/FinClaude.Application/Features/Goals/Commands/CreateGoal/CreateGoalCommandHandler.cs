using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Goals.DTOs;

namespace FinClaude.Application.Features.Goals.Commands.CreateGoal;

public class CreateGoalCommandHandler(
    IChainProvider<CreateGoalContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<CreateGoalCommand, GoalResponse>
{
    public async Task<ErrorOr<GoalResponse>> HandleAsync(CreateGoalCommand command, CancellationToken ct = default)
    {
        var context = new CreateGoalContext
        {
            AccountId = command.AccountId,
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
        var g = context.CreatedGoal!;
        return new GoalResponse(g.Id, g.Name, g.TargetValue, g.TargetDate, g.AssetId, g.GroupId, null, null, g.CreatedAt);
    }
}
