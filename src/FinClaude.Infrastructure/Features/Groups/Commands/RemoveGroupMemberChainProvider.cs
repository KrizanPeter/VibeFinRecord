using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.RemoveGroupMember;
using FinClaude.Infrastructure.Features.Groups.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Groups.Commands;

public class RemoveGroupMemberChainProvider(
    AuthorizeAndRemoveMemberStep authorizeAndRemove) : IChainProvider<RemoveGroupMemberContext>
{
    public IStep<RemoveGroupMemberContext> GetChain() => authorizeAndRemove;
}
