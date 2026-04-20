using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Groups.DTOs;

namespace FinClaude.Application.Features.Groups.Queries.ListGroups;

public record ListGroupsQuery(Guid AccountId) : IQuery<List<AssetGroupResponse>>;
