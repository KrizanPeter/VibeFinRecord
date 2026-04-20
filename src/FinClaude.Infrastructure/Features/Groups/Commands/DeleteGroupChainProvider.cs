using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.DeleteGroup;
using FinClaude.Infrastructure.Features.Groups.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Groups.Commands;

public class DeleteGroupChainProvider(
    AuthorizeAndDeleteGroupStep authorizeAndDelete) : IChainProvider<DeleteGroupContext>
{
    public IStep<DeleteGroupContext> GetChain() => authorizeAndDelete;
}
