using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Groups.DTOs;

namespace FinClaude.Application.Features.Groups.Commands.CreateGroup;

public class CreateGroupCommandHandler(
    IChainProvider<CreateGroupContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<CreateGroupCommand, AssetGroupResponse>
{
    public async Task<ErrorOr<AssetGroupResponse>> HandleAsync(CreateGroupCommand command, CancellationToken ct = default)
    {
        var context = new CreateGroupContext { AccountId = command.AccountId, Name = command.Name };
        var chain = chainProvider.GetChain();

        await uow.BeginAsync(ct);
        var result = await chain.ExecuteAsync(context, ct);

        if (result.IsError)
        {
            await uow.RollbackAsync(ct);
            return result.Errors;
        }

        await uow.CommitAsync(ct);
        var g = context.CreatedGroup!;
        return new AssetGroupResponse(g.Id, g.Name, g.CreatedAt);
    }
}
