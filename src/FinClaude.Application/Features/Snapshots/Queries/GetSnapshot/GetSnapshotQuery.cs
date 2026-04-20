using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Snapshots.DTOs;

namespace FinClaude.Application.Features.Snapshots.Queries.GetSnapshot;

public record GetSnapshotQuery(Guid AccountId, Guid SnapshotId) : IQuery<SnapshotDetailResponse>;
