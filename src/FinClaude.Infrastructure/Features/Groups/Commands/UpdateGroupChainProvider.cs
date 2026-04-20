using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.UpdateGroup;
using FinClaude.Infrastructure.Features.Groups.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Groups.Commands;

public class UpdateGroupChainProvider(
    ValidateUpdateGroupStep validate,
    AuthorizeAndLoadGroupStep authorizeAndLoad,
    PersistUpdateGroupStep persist) : IChainProvider<UpdateGroupContext>
{
    public IStep<UpdateGroupContext> GetChain()
    {
        validate.SetNext(authorizeAndLoad).SetNext(persist);
        return validate;
    }
}
