using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Dashboards.DTOs;

namespace FinClaude.Application.Features.Dashboards.Commands.UpdateDashboardChart;

public class UpdateDashboardChartCommandHandler(
    IChainProvider<UpdateDashboardChartContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<UpdateDashboardChartCommand, DashboardChartResponse>
{
    public async Task<ErrorOr<DashboardChartResponse>> HandleAsync(UpdateDashboardChartCommand command, CancellationToken ct = default)
    {
        var context = new UpdateDashboardChartContext
        {
            AccountId = command.AccountId,
            ChartId = command.ChartId,
            Name = command.Name,
            ChartType = command.ChartType,
            SourceType = command.SourceType,
            AssetId = command.AssetId,
            GroupId = command.GroupId,
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
        var c = context.Chart!;
        return new DashboardChartResponse(c.Id, c.Name, c.ChartType, c.SourceType, c.AssetId, c.GroupId, c.CreatedAt);
    }
}
