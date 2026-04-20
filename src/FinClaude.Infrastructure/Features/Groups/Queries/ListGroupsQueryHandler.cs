using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Groups.DTOs;
using FinClaude.Application.Features.Groups.Queries.ListGroups;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Groups.Queries;

public class ListGroupsQueryHandler(AppDbContext db) : IQueryHandler<ListGroupsQuery, List<AssetGroupResponse>>
{
    public async Task<ErrorOr<List<AssetGroupResponse>>> HandleAsync(ListGroupsQuery query, CancellationToken ct = default)
    {
        var groups = await db.AssetGroups
            .AsNoTracking()
            .Where(g => g.AccountId == query.AccountId)
            .OrderBy(g => g.Name)
            .Select(g => new AssetGroupResponse(g.Id, g.Name, g.CreatedAt))
            .ToListAsync(ct);

        return groups;
    }
}
