using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Auth.Register;
using FinClaude.Infrastructure.Features.Auth.Register.Steps;

namespace FinClaude.Infrastructure.Features.Auth.Register;

public class RegisterChainProvider(
    CreateIdentityUserStep createUser,
    CreateAccountStep createAccount,
    GenerateRegisterTokensStep generateTokens) : IChainProvider<RegisterContext>
{
    public IStep<RegisterContext> GetChain()
    {
        createUser.SetNext(createAccount).SetNext(generateTokens);
        return createUser;
    }
}
