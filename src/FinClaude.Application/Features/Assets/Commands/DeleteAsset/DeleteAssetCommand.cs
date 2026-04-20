using FinClaude.Application.Common.CQRS;

namespace FinClaude.Application.Features.Assets.Commands.DeleteAsset;

public record DeleteAssetCommand(Guid AccountId, Guid AssetId) : ICommand;
