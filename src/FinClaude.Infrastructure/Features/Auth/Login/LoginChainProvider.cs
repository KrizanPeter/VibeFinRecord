using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Auth.Login;
using FinClaude.Infrastructure.Features.Auth.Login.Steps;

namespace FinClaude.Infrastructure.Features.Auth.Login;

public class LoginChainProvider(
    ValidateCredentialsStep validateCredentials,
    GenerateLoginTokensStep generateTokens) : IChainProvider<LoginContext>
{
    public IStep<LoginContext> GetChain()
    {
        validateCredentials.SetNext(generateTokens);
        return validateCredentials;
    }
}
