using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;

namespace FinClaude.Application.Features.Assets.Commands.DeleteAsset;

public class DeleteAssetCommandHandler(
    IChainProvider<DeleteAssetContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<DeleteAssetCommand>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteAssetCommand command, CancellationToken ct = default)
    {
        var context = new DeleteAssetContext { AccountId = command.AccountId, AssetId = command.AssetId };
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
