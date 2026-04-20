using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Assets.DTOs;
using FinClaude.Application.Features.Assets.Queries.ListAssets;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Assets.Queries;

public class ListAssetsQueryHandler(AppDbContext db) : IQueryHandler<ListAssetsQuery, List<AssetResponse>>
{
    public async Task<ErrorOr<List<AssetResponse>>> HandleAsync(ListAssetsQuery query, CancellationToken ct = default)
    {
        var assets = await db.Assets
            .AsNoTracking()
            .Where(a => a.AccountId == query.AccountId)
            .OrderBy(a => a.Name)
            .Select(a => new AssetResponse(a.Id, a.Name, a.Institution, a.CreatedAt))
            .ToListAsync(ct);

        return assets;
    }
}
