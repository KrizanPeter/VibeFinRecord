using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Assets.DTOs;

namespace FinClaude.Application.Features.Assets.Commands.CreateAsset;

public class CreateAssetCommandHandler(
    IChainProvider<CreateAssetContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<CreateAssetCommand, AssetResponse>
{
    public async Task<ErrorOr<AssetResponse>> HandleAsync(CreateAssetCommand command, CancellationToken ct = default)
    {
        var context = new CreateAssetContext { AccountId = command.AccountId, Name = command.Name, Institution = command.Institution };
        var chain = chainProvider.GetChain();

        await uow.BeginAsync(ct);
        var result = await chain.ExecuteAsync(context, ct);

        if (result.IsError)
        {
            await uow.RollbackAsync(ct);
            return result.Errors;
        }

        await uow.CommitAsync(ct);
        var a = context.CreatedAsset!;
        return new AssetResponse(a.Id, a.Name, a.Institution, a.CreatedAt);
    }
}
