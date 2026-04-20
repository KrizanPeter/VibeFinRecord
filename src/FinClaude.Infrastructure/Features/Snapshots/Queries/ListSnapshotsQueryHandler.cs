using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.DTOs;
using FinClaude.Application.Features.Snapshots.DTOs;
using FinClaude.Application.Features.Snapshots.Queries.ListSnapshots;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Snapshots.Queries;

public class ListSnapshotsQueryHandler(AppDbContext db) : IQueryHandler<ListSnapshotsQuery, PagedResponse<SnapshotSummaryResponse>>
{
    public async Task<ErrorOr<PagedResponse<SnapshotSummaryResponse>>> HandleAsync(ListSnapshotsQuery query, CancellationToken ct = default)
    {
        var baseQuery = db.Snapshots
            .AsNoTracking()
            .Where(s => s.AccountId == query.AccountId);

        var totalCount = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            .OrderByDescending(s => s.SnapshotDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .GroupJoin(
                db.AssetSnapshots,
                s => s.Id,
                a => a.SnapshotId,
                (s, assetSnaps) => new SnapshotSummaryResponse(
                    s.Id,
                    s.SnapshotDate,
                    assetSnaps.Sum(a => (decimal?)a.Value) ?? 0m,
                    s.CreatedAt))
            .ToListAsync(ct);

        return new PagedResponse<SnapshotSummaryResponse>(items, totalCount, query.Page, query.PageSize);
    }
}
