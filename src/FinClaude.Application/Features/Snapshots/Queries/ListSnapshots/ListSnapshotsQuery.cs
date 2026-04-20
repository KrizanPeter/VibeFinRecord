using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.DTOs;
using FinClaude.Application.Features.Snapshots.DTOs;

namespace FinClaude.Application.Features.Snapshots.Queries.ListSnapshots;

public record ListSnapshotsQuery(Guid AccountId, int Page, int PageSize) : IQuery<PagedResponse<SnapshotSummaryResponse>>;
