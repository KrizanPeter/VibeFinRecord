using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Snapshots.DTOs;

namespace FinClaude.Application.Features.Snapshots.Queries.GetSnapshotStatus;

public record GetSnapshotStatusQuery(Guid AccountId) : IQuery<SnapshotStatusResponse>;
