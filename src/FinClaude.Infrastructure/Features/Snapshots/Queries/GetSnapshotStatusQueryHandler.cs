using ErrorOr;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Snapshots.DTOs;
using FinClaude.Application.Features.Snapshots.Queries.GetSnapshotStatus;
using FinClaude.Domain.Enums;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Snapshots.Queries;

public class GetSnapshotStatusQueryHandler(AppDbContext db) : IQueryHandler<GetSnapshotStatusQuery, SnapshotStatusResponse>
{
    public async Task<ErrorOr<SnapshotStatusResponse>> HandleAsync(GetSnapshotStatusQuery query, CancellationToken ct = default)
    {
        var account = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == query.AccountId, ct);

        if (account is null)
            return Error.NotFound("Account.NotFound", "Account not found.");

        if (account.SnapshotStartDate is null || account.SnapshotPeriodicity is null)
            return Error.Validation("Account.SetupIncomplete", "Account setup is not complete.");

        var existingDates = await db.Snapshots
            .AsNoTracking()
            .Where(s => s.AccountId == query.AccountId)
            .Select(s => s.SnapshotDate)
            .ToListAsync(ct);

        var existingSet = existingDates.ToHashSet();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var expectedDates = GenerateExpectedDates(account.SnapshotStartDate.Value, account.SnapshotPeriodicity.Value, today);
        var missingDates = expectedDates.Where(d => !existingSet.Contains(d)).ToList();
        var nextExpected = GetNextExpectedDate(account.SnapshotStartDate.Value, account.SnapshotPeriodicity.Value, today);

        return new SnapshotStatusResponse(missingDates, nextExpected);
    }

    private static List<DateOnly> GenerateExpectedDates(DateOnly start, SnapshotPeriodicity periodicity, DateOnly today)
    {
        var dates = new List<DateOnly>();
        var originalDay = start.Day;
        var current = start;

        while (current <= today)
        {
            dates.Add(current);
            current = NextDate(current, originalDay, periodicity);
        }

        return dates;
    }

    private static DateOnly? GetNextExpectedDate(DateOnly start, SnapshotPeriodicity periodicity, DateOnly today)
    {
        var originalDay = start.Day;
        var current = start;

        while (current <= today)
            current = NextDate(current, originalDay, periodicity);

        return current;
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

        var firstOfNextPeriod = new DateOnly(current.Year, current.Month, 1).AddMonths(monthsToAdd);
        var daysInMonth = DateTime.DaysInMonth(firstOfNextPeriod.Year, firstOfNextPeriod.Month);
        return new DateOnly(firstOfNextPeriod.Year, firstOfNextPeriod.Month, Math.Min(originalDay, daysInMonth));
    }
}
