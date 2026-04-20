using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Assets.Commands.UpdateAsset;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Assets.Commands.Steps;

public class AuthorizeAndLoadAssetStep(AppDbContext db) : BaseStep<UpdateAssetContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateAssetContext context, CancellationToken ct = default)
    {
        var asset = await db.Assets.FirstOrDefaultAsync(
            a => a.Id == context.AssetId && a.AccountId == context.AccountId, ct);

        if (asset is null)
            return Error.NotFound("Asset.NotFound", "Asset not found.");

        context.Asset = asset;
        return await NextAsync(context, ct);
    }
}
