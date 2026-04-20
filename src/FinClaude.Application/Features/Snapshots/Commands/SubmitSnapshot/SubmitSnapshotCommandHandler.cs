using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Snapshots.DTOs;

namespace FinClaude.Application.Features.Snapshots.Commands.SubmitSnapshot;

public class SubmitSnapshotCommandHandler(
    IChainProvider<SubmitSnapshotContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<SubmitSnapshotCommand, SnapshotDetailResponse>
{
    public async Task<ErrorOr<SnapshotDetailResponse>> HandleAsync(SubmitSnapshotCommand command, CancellationToken ct = default)
    {
        var context = new SubmitSnapshotContext
        {
            AccountId = command.AccountId,
            SnapshotDate = command.SnapshotDate,
            Assets = command.Assets
        };
        var chain = chainProvider.GetChain();

        await uow.BeginAsync(ct);
        var result = await chain.ExecuteAsync(context, ct);

        if (result.IsError)
        {
            await uow.RollbackAsync(ct);
            return result.Errors;
        }

        await uow.CommitAsync(ct);

        var snap = context.CreatedSnapshot!;
        var assetNames = context.ActiveAssets.ToDictionary(a => a.Id, a => a.Name);
        var assetEntries = command.Assets
            .Select(a => new AssetSnapshotEntry(a.AssetId, assetNames[a.AssetId], a.Value))
            .ToList();

        return new SnapshotDetailResponse(snap.Id, snap.SnapshotDate, assetEntries, snap.CreatedAt);
    }
}
