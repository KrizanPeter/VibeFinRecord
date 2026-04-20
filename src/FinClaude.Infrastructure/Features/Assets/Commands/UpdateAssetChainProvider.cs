using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Assets.Commands.UpdateAsset;
using FinClaude.Infrastructure.Features.Assets.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Assets.Commands;

public class UpdateAssetChainProvider(
    ValidateUpdateAssetStep validate,
    AuthorizeAndLoadAssetStep authorizeAndLoad,
    PersistUpdateAssetStep persist) : IChainProvider<UpdateAssetContext>
{
    public IStep<UpdateAssetContext> GetChain()
    {
        validate.SetNext(authorizeAndLoad).SetNext(persist);
        return validate;
    }
}
