using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Snapshots.DTOs;

namespace FinClaude.Application.Features.Snapshots.Commands.SubmitSnapshot;

public record AssetValueInput(Guid AssetId, decimal Value);

public record SubmitSnapshotCommand(
    Guid AccountId,
    DateOnly SnapshotDate,
    List<AssetValueInput> Assets) : ICommand<SnapshotDetailResponse>;
