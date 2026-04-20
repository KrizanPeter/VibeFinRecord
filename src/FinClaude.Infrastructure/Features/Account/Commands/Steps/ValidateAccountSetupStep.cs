using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Account.Commands.UpdateAccount;
using System.Text.RegularExpressions;

namespace FinClaude.Infrastructure.Features.Account.Commands.Steps;

public class ValidateAccountSetupStep : BaseStep<UpdateAccountContext>
{
    private static readonly Regex CurrencyRegex = new(@"^[A-Z]{3}$", RegexOptions.Compiled);

    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateAccountContext context, CancellationToken ct = default)
    {
        if (!CurrencyRegex.IsMatch(context.Currency))
            return Error.Validation("Account.InvalidCurrency", "Currency must be a 3-letter ISO 4217 code (e.g. USD).");

        if (context.SnapshotStartDate > DateOnly.FromDateTime(DateTime.UtcNow))
            return Error.Validation("Account.FutureStartDate", "Snapshot start date cannot be in the future.");

        return await NextAsync(context, ct);
    }
}
