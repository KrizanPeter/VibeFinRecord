using ErrorOr;
using FinClaude.Application.Common.Auth;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Auth.Register;

namespace FinClaude.Infrastructure.Features.Auth.Register.Steps;

public class GenerateRegisterTokensStep(ITokenService tokenService) : BaseStep<RegisterContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(RegisterContext context, CancellationToken ct = default)
    {
        context.AccessToken = tokenService.GenerateAccessToken(context.IdentityUserId, context.AccountId, context.Email);
        context.RefreshToken = tokenService.GenerateRefreshToken();
        await tokenService.StoreRefreshTokenAsync(context.IdentityUserId, context.RefreshToken);

        return await NextAsync(context, ct);
    }
}
