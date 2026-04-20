using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Assets.DTOs;

namespace FinClaude.Application.Features.Assets.Commands.CreateAsset;

public record CreateAssetCommand(Guid AccountId, string Name, string? Institution) : ICommand<AssetResponse>;
