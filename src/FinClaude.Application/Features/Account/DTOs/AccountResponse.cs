using FinClaude.Domain.Enums;

namespace FinClaude.Application.Features.Account.DTOs;

public record AccountResponse(
    Guid Id,
    string? Currency,
    DateOnly? SnapshotStartDate,
    SnapshotPeriodicity? SnapshotPeriodicity,
    bool IsSetupComplete);
