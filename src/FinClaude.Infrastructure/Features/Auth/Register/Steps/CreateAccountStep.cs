using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Auth.Register;
using FinClaude.Infrastructure.Persistence;
using AccountEntity = FinClaude.Domain.Entities.Account;

namespace FinClaude.Infrastructure.Features.Auth.Register.Steps;

public class CreateAccountStep(AppDbContext db) : BaseStep<RegisterContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(RegisterContext context, CancellationToken ct = default)
    {
        var account = new AccountEntity { IdentityUserId = context.IdentityUserId };
        db.Accounts.Add(account);
        context.AccountId = account.Id;

        return await NextAsync(context, ct);
    }
}
