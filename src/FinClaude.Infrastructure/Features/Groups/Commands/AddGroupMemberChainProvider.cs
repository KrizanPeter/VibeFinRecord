using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.AddGroupMember;
using FinClaude.Infrastructure.Features.Groups.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Groups.Commands;

public class AddGroupMemberChainProvider(
    ValidateAndPersistAddMemberStep validateAndPersist) : IChainProvider<AddGroupMemberContext>
{
    public IStep<AddGroupMemberContext> GetChain() => validateAndPersist;
}
