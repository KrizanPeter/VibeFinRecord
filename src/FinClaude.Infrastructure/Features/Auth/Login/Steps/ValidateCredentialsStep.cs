using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Auth;
using FinClaude.Application.Features.Auth.Login;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Auth.Login.Steps;

public class ValidateCredentialsStep(IIdentityService identityService, AppDbContext db) : BaseStep<LoginContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(LoginContext context, CancellationToken ct = default)
    {
        var credsResult = await identityService.ValidateCredentialsAsync(context.Email, context.Password);
        if (credsResult.IsError)
            return credsResult.Errors;

        var (userId, email) = credsResult.Value;
        context.IdentityUserId = userId;
        context.UserEmail = email;

        var account = await db.Accounts.FirstOrDefaultAsync(a => a.IdentityUserId == userId, ct);
        if (account is null)
            return Error.NotFound("Auth.AccountNotFound", "Account not found.");

        context.AccountId = account.Id;
        return await NextAsync(context, ct);
    }
}
