using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Assets.Commands.CreateAsset;
using FinClaude.Infrastructure.Features.Assets.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Assets.Commands;

public class CreateAssetChainProvider(
    ValidateCreateAssetStep validate,
    PersistCreateAssetStep persist) : IChainProvider<CreateAssetContext>
{
    public IStep<CreateAssetContext> GetChain()
    {
        validate.SetNext(persist);
        return validate;
    }
}
