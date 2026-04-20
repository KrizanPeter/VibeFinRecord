using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Assets.DTOs;

namespace FinClaude.Application.Features.Assets.Queries.GetAsset;

public record GetAssetQuery(Guid AccountId, Guid AssetId) : IQuery<AssetResponse>;
