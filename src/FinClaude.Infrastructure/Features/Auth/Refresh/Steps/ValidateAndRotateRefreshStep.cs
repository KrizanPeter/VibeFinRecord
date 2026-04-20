using ErrorOr;
using FinClaude.Application.Common.Auth;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Auth;
using FinClaude.Application.Features.Auth.Refresh;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Auth.Refresh.Steps;

public class ValidateAndRotateRefreshStep(
    IIdentityService identityService,
    ITokenService tokenService,
    AppDbContext db) : BaseStep<RefreshContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(RefreshContext context, CancellationToken ct = default)
    {
        var userResult = await identityService.FindUserByIdAsync(context.UserId);
        if (userResult.IsError)
            return Error.Validation("Auth.InvalidToken", "Invalid refresh token.");

        var (userId, email) = userResult.Value;
        context.UserEmail = email;

        var account = await db.Accounts.FirstOrDefaultAsync(a => a.IdentityUserId == userId, ct);
        if (account is null)
            return Error.NotFound("Auth.AccountNotFound", "Account not found.");

        context.AccountId = account.Id;

        var newRefreshToken = tokenService.GenerateRefreshToken();
        var rotated = await tokenService.ValidateAndRotateRefreshTokenAsync(userId, context.CurrentRefreshToken, newRefreshToken);
        if (!rotated)
            return Error.Validation("Auth.InvalidToken", "Invalid or expired refresh token.");

        context.NewRefreshToken = newRefreshToken;
        context.AccessToken = tokenService.GenerateAccessToken(userId, account.Id, email);

        return await NextAsync(context, ct);
    }
}
