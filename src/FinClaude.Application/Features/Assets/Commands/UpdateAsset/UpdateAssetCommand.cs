using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Assets.DTOs;

namespace FinClaude.Application.Features.Assets.Commands.UpdateAsset;

public record UpdateAssetCommand(Guid AccountId, Guid AssetId, string Name, string? Institution) : ICommand<AssetResponse>;
