using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Snapshots.Commands.SubmitSnapshot;
using FinClaude.Domain.Entities;
using FinClaude.Infrastructure.Persistence;

namespace FinClaude.Infrastructure.Features.Snapshots.Commands.Steps;

public class PersistSnapshotStep(AppDbContext db) : BaseStep<SubmitSnapshotContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(SubmitSnapshotContext context, CancellationToken ct = default)
    {
        var snapshot = new Snapshot { AccountId = context.AccountId, SnapshotDate = context.SnapshotDate };
        db.Snapshots.Add(snapshot);

        foreach (var assetValue in context.Assets)
        {
            db.AssetSnapshots.Add(new AssetSnapshot
            {
                SnapshotId = snapshot.Id,
                AssetId = assetValue.AssetId,
                Value = assetValue.Value
            });
        }

        context.CreatedSnapshot = snapshot;
        return await NextAsync(context, ct);
    }
}
