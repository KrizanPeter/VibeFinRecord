using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Account.DTOs;
using FinClaude.Domain.Enums;

namespace FinClaude.Application.Features.Account.Commands.UpdateAccount;

public record UpdateAccountCommand(
    Guid AccountId,
    string Currency,
    DateOnly SnapshotStartDate,
    SnapshotPeriodicity SnapshotPeriodicity) : ICommand<AccountResponse>;
