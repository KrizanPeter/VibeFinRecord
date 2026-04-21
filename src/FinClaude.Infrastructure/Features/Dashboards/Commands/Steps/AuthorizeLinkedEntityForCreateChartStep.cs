using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Dashboards.Commands.CreateDashboardChart;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Dashboards.Commands.Steps;

public class AuthorizeLinkedEntityForCreateChartStep(AppDbContext db) : BaseStep<CreateDashboardChartContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateDashboardChartContext context, CancellationToken ct = default)
    {
        if (context.AssetId.HasValue)
        {
            var exists = await db.Assets.AnyAsync(
                a => a.Id == context.AssetId && a.AccountId == context.AccountId, ct);
            if (!exists)
                return Error.NotFound("Asset.NotFound", "Asset not found.");
        }
        else
        {
            var exists = await db.AssetGroups.AnyAsync(
                g => g.Id == context.GroupId && g.AccountId == context.AccountId, ct);
            if (!exists)
                return Error.NotFound("AssetGroup.NotFound", "Asset group not found.");
        }

        return await NextAsync(context, ct);
    }
}
