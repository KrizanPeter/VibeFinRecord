using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Groups.DTOs;

namespace FinClaude.Application.Features.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandHandler(
    IChainProvider<UpdateGroupContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<UpdateGroupCommand, AssetGroupResponse>
{
    public async Task<ErrorOr<AssetGroupResponse>> HandleAsync(UpdateGroupCommand command, CancellationToken ct = default)
    {
        var context = new UpdateGroupContext { AccountId = command.AccountId, GroupId = command.GroupId, Name = command.Name };
        var chain = chainProvider.GetChain();

        await uow.BeginAsync(ct);
        var result = await chain.ExecuteAsync(context, ct);

        if (result.IsError)
        {
            await uow.RollbackAsync(ct);
            return result.Errors;
        }

        await uow.CommitAsync(ct);
        var g = context.Group!;
        return new AssetGroupResponse(g.Id, g.Name, g.CreatedAt);
    }
}
