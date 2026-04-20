using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Groups.DTOs;

namespace FinClaude.Application.Features.Groups.Queries.GetGroup;

public record GetGroupQuery(Guid AccountId, Guid GroupId) : IQuery<AssetGroupDetailResponse>;
