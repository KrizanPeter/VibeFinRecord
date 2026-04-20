using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Snapshots.Commands.SubmitSnapshot;
using FinClaude.Infrastructure.Features.Snapshots.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Snapshots.Commands;

public class SubmitSnapshotChainProvider(
    ValidateSnapshotSubmitStep validate,
    PersistSnapshotStep persist) : IChainProvider<SubmitSnapshotContext>
{
    public IStep<SubmitSnapshotContext> GetChain()
    {
        validate.SetNext(persist);
        return validate;
    }
}
