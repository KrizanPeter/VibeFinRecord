using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Snapshots.DTOs;
using FinClaude.Application.Features.Snapshots.Queries.GetSnapshot;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Snapshots.Queries;

public class GetSnapshotQueryHandler(AppDbContext db) : IQueryHandler<GetSnapshotQuery, SnapshotDetailResponse>
{
    public async Task<ErrorOr<SnapshotDetailResponse>> HandleAsync(GetSnapshotQuery query, CancellationToken ct = default)
    {
        var snapshot = await db.Snapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == query.SnapshotId && s.AccountId == query.AccountId, ct);

        if (snapshot is null)
            return Error.NotFound("Snapshot.NotFound", "Snapshot not found.");

        var assets = await db.AssetSnapshots
            .AsNoTracking()
            .Where(a => a.SnapshotId == snapshot.Id)
            .Join(db.Assets, a => a.AssetId, asset => asset.Id,
                (a, asset) => new AssetSnapshotEntry(a.AssetId, asset.Name, a.Value))
            .ToListAsync(ct);

        return new SnapshotDetailResponse(snapshot.Id, snapshot.SnapshotDate, assets, snapshot.CreatedAt);
    }
}
