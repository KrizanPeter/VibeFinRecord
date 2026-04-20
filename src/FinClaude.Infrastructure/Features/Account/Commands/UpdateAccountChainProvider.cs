using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Account.Commands.UpdateAccount;
using FinClaude.Infrastructure.Features.Account.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Account.Commands;

public class UpdateAccountChainProvider(
    ValidateAccountSetupStep validate,
    LockIfSnapshotsExistStep lockCheck,
    PersistAccountSetupStep persist) : IChainProvider<UpdateAccountContext>
{
    public IStep<UpdateAccountContext> GetChain()
    {
        validate.SetNext(lockCheck).SetNext(persist);
        return validate;
    }
}
