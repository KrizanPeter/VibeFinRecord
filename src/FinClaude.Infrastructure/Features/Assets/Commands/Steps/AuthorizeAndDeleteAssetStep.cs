using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Assets.Commands.DeleteAsset;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Assets.Commands.Steps;

public class AuthorizeAndDeleteAssetStep(AppDbContext db) : BaseStep<DeleteAssetContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(DeleteAssetContext context, CancellationToken ct = default)
    {
        var asset = await db.Assets.FirstOrDefaultAsync(
            a => a.Id == context.AssetId && a.AccountId == context.AccountId, ct);

        if (asset is null)
            return Error.NotFound("Asset.NotFound", "Asset not found.");

        var now = DateTime.UtcNow;
        asset.DeletedAt = now;

        // Cascade soft-delete linked Goals and DashboardCharts
        var goals = await db.Goals.Where(g => g.AssetId == context.AssetId).ToListAsync(ct);
        foreach (var goal in goals)
            goal.DeletedAt = now;

        var charts = await db.DashboardCharts.Where(c => c.AssetId == context.AssetId).ToListAsync(ct);
        foreach (var chart in charts)
            chart.DeletedAt = now;

        return await NextAsync(context, ct);
    }
}
