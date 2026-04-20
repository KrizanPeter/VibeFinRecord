using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Groups.DTOs;
using FinClaude.Application.Features.Groups.Queries.GetGroup;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Groups.Queries;

public class GetGroupQueryHandler(AppDbContext db) : IQueryHandler<GetGroupQuery, AssetGroupDetailResponse>
{
    public async Task<ErrorOr<AssetGroupDetailResponse>> HandleAsync(GetGroupQuery query, CancellationToken ct = default)
    {
        var group = await db.AssetGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == query.GroupId && g.AccountId == query.AccountId, ct);

        if (group is null)
            return Error.NotFound("Group.NotFound", "Asset group not found.");

        var memberAssetIds = await db.AssetGroupMemberships
            .AsNoTracking()
            .Where(m => m.GroupId == query.GroupId)
            .Select(m => m.AssetId)
            .ToListAsync(ct);

        return new AssetGroupDetailResponse(group.Id, group.Name, group.CreatedAt, memberAssetIds);
    }
}
