using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Goals.Commands.CreateGoal;
using FinClaude.Infrastructure.Features.Goals.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Goals.Commands;

public class CreateGoalChainProvider(
    ValidateCreateGoalStep validate,
    AuthorizeLinkedEntityForCreateStep authorizeLinked,
    PersistCreateGoalStep persist) : IChainProvider<CreateGoalContext>
{
    public IStep<CreateGoalContext> GetChain()
    {
        validate.SetNext(authorizeLinked).SetNext(persist);
        return validate;
    }
}
