using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Goals.Commands.UpdateGoal;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Goals.Commands.Steps;

public class AuthorizeLinkedEntityForUpdateStep(AppDbContext db) : BaseStep<UpdateGoalContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateGoalContext context, CancellationToken ct = default)
    {
        if (context.AssetId.HasValue)
        {
            var exists = await db.Assets.AnyAsync(
                a => a.Id == context.AssetId && a.AccountId == context.AccountId && a.DeletedAt == null, ct);
            if (!exists)
                return Error.NotFound("Asset.NotFound", "Asset not found.");
        }
        else
        {
            var exists = await db.AssetGroups.AnyAsync(
                g => g.Id == context.GroupId && g.AccountId == context.AccountId && g.DeletedAt == null, ct);
            if (!exists)
                return Error.NotFound("AssetGroup.NotFound", "Asset group not found.");
        }

        return await NextAsync(context, ct);
    }
}
