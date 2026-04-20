using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Assets.Commands.CreateAsset;
using FinClaude.Domain.Entities;
using FinClaude.Infrastructure.Persistence;

namespace FinClaude.Infrastructure.Features.Assets.Commands.Steps;

public class PersistCreateAssetStep(AppDbContext db) : BaseStep<CreateAssetContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateAssetContext context, CancellationToken ct = default)
    {
        var asset = new Asset { AccountId = context.AccountId, Name = context.Name, Institution = context.Institution };
        db.Assets.Add(asset);
        context.CreatedAsset = asset;

        return await NextAsync(context, ct);
    }
}
