using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Goals.Commands.DeleteGoal;
using FinClaude.Infrastructure.Features.Goals.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Goals.Commands;

public class DeleteGoalChainProvider(
    AuthorizeAndDeleteGoalStep authorizeAndDelete) : IChainProvider<DeleteGoalContext>
{
    public IStep<DeleteGoalContext> GetChain() => authorizeAndDelete;
}
