using FinClaude.Domain.Enums;

namespace FinClaude.Application.Features.Account.Commands.UpdateAccount;

public class UpdateAccountContext
{
    public required Guid AccountId { get; init; }
    public required string Currency { get; init; }
    public required DateOnly SnapshotStartDate { get; init; }
    public required SnapshotPeriodicity SnapshotPeriodicity { get; init; }
    public Domain.Entities.Account? Account { get; set; }
}
