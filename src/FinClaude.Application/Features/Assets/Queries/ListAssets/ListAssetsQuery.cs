using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Assets.DTOs;

namespace FinClaude.Application.Features.Assets.Queries.ListAssets;

public record ListAssetsQuery(Guid AccountId) : IQuery<List<AssetResponse>>;
