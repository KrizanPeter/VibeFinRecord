using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Assets.Commands.DeleteAsset;
using FinClaude.Infrastructure.Features.Assets.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Assets.Commands;

public class DeleteAssetChainProvider(
    AuthorizeAndDeleteAssetStep authorizeAndDelete) : IChainProvider<DeleteAssetContext>
{
    public IStep<DeleteAssetContext> GetChain() => authorizeAndDelete;
}
