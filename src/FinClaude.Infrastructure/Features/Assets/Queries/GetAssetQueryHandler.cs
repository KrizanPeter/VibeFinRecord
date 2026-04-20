using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Assets.DTOs;
using FinClaude.Application.Features.Assets.Queries.GetAsset;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Assets.Queries;

public class GetAssetQueryHandler(AppDbContext db) : IQueryHandler<GetAssetQuery, AssetResponse>
{
    public async Task<ErrorOr<AssetResponse>> HandleAsync(GetAssetQuery query, CancellationToken ct = default)
    {
        var asset = await db.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == query.AssetId && a.AccountId == query.AccountId, ct);

        if (asset is null)
            return Error.NotFound("Asset.NotFound", "Asset not found.");

        return new AssetResponse(asset.Id, asset.Name, asset.Institution, asset.CreatedAt);
    }
}
