using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Assets.Commands.UpdateAsset;

namespace FinClaude.Infrastructure.Features.Assets.Commands.Steps;

public class PersistUpdateAssetStep : BaseStep<UpdateAssetContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateAssetContext context, CancellationToken ct = default)
    {
        var asset = context.Asset!;
        asset.Name = context.Name;
        asset.Institution = context.Institution;

        return await NextAsync(context, ct);
    }
}
