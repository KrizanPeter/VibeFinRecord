using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Goals.Commands.UpdateGoal;
using FinClaude.Infrastructure.Features.Goals.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Goals.Commands;

public class UpdateGoalChainProvider(
    ValidateUpdateGoalStep validate,
    AuthorizeAndLoadGoalStep authorizeAndLoad,
    AuthorizeLinkedEntityForUpdateStep authorizeLinked,
    PersistUpdateGoalStep persist) : IChainProvider<UpdateGoalContext>
{
    public IStep<UpdateGoalContext> GetChain()
    {
        validate.SetNext(authorizeAndLoad).SetNext(authorizeLinked).SetNext(persist);
        return validate;
    }
}
