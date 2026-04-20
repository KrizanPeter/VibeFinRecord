using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.CreateGroup;
using FinClaude.Infrastructure.Features.Groups.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Groups.Commands;

public class CreateGroupChainProvider(
    ValidateCreateGroupStep validate,
    PersistCreateGroupStep persist) : IChainProvider<CreateGroupContext>
{
    public IStep<CreateGroupContext> GetChain()
    {
        validate.SetNext(persist);
        return validate;
    }
}
