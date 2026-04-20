using ErrorOr;
using FinClaude.Application.Common.Auth;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Auth.Login;

namespace FinClaude.Infrastructure.Features.Auth.Login.Steps;

public class GenerateLoginTokensStep(ITokenService tokenService) : BaseStep<LoginContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(LoginContext context, CancellationToken ct = default)
    {
        context.AccessToken = tokenService.GenerateAccessToken(context.IdentityUserId, context.AccountId, context.UserEmail);
        context.RefreshToken = tokenService.GenerateRefreshToken();
        await tokenService.StoreRefreshTokenAsync(context.IdentityUserId, context.RefreshToken);

        return await NextAsync(context, ct);
    }
}
