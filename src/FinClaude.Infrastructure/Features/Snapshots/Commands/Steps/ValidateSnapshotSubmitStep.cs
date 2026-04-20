using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Snapshots.Commands.SubmitSnapshot;
using FinClaude.Domain.Enums;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Snapshots.Commands.Steps;

public class ValidateSnapshotSubmitStep(AppDbContext db) : BaseStep<SubmitSnapshotContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(SubmitSnapshotContext context, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (context.SnapshotDate > today)
            return Error.Failure("Snapshot.FutureDateNotAllowed", "Snapshot date cannot be in the future.");

        var exists = await db.Snapshots.AnyAsync(
            s => s.AccountId == context.AccountId && s.SnapshotDate == context.SnapshotDate, ct);
        if (exists)
            return Error.Conflict("Snapshot.AlreadyExists", "A snapshot already exists for this date.");

        var account = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == context.AccountId, ct);

        if (account?.SnapshotStartDate is null || account.SnapshotPeriodicity is null)
            return Error.Validation("Account.SetupIncomplete", "Account snapshot setup is not complete.");

        var expectedDates = GenerateExpectedDates(account.SnapshotStartDate.Value, account.SnapshotPeriodicity.Value, today);
        if (!expectedDates.Contains(context.SnapshotDate))
            return Error.Validation("Snapshot.InvalidDate", "The snapshot date is not a valid expected snapshot date.");

        var activeAssets = await db.Assets
            .AsNoTracking()
            .Where(a => a.AccountId == context.AccountId && a.DeletedAt == null)
            .ToListAsync(ct);

        var submittedIds = context.Assets.Select(a => a.AssetId).ToHashSet();
        var activeIds = activeAssets.Select(a => a.Id).ToHashSet();

        if (!activeIds.SetEquals(submittedIds))
            return Error.Validation("Snapshot.AssetCoverageIncomplete", "All active assets must be included in the snapshot.");

        context.ActiveAssets = activeAssets;
        return await NextAsync(context, ct);
    }

    private static HashSet<DateOnly> GenerateExpectedDates(DateOnly start, SnapshotPeriodicity periodicity, DateOnly today)
    {
        var dates = new HashSet<DateOnly>();
        var originalDay = start.Day;
        var current = start;
        while (current <= today)
        {
            dates.Add(current);
            current = NextDate(current, originalDay, periodicity);
        }
        return dates;
    }

    private static DateOnly NextDate(DateOnly current, int originalDay, SnapshotPeriodicity periodicity)
    {
        var monthsToAdd = periodicity switch
        {
            SnapshotPeriodicity.Monthly => 1,
            SnapshotPeriodicity.Quarterly => 3,
            SnapshotPeriodicity.Yearly => 12,
            _ => throw new ArgumentOutOfRangeException(nameof(periodicity))
        };
        var first = new DateOnly(current.Year, current.Month, 1).AddMonths(monthsToAdd);
        var daysInMonth = DateTime.DaysInMonth(first.Year, first.Month);
        return new DateOnly(first.Year, first.Month, Math.Min(originalDay, daysInMonth));
    }
}
