using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Dashboards.DTOs;

namespace FinClaude.Application.Features.Dashboards.Commands.CreateDashboardChart;

public class CreateDashboardChartCommandHandler(
    IChainProvider<CreateDashboardChartContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<CreateDashboardChartCommand, DashboardChartResponse>
{
    public async Task<ErrorOr<DashboardChartResponse>> HandleAsync(CreateDashboardChartCommand command, CancellationToken ct = default)
    {
        var context = new CreateDashboardChartContext
        {
            AccountId = command.AccountId,
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
        var c = context.CreatedChart!;
        return new DashboardChartResponse(c.Id, c.Name, c.ChartType, c.SourceType, c.AssetId, c.GroupId, c.CreatedAt);
    }
}
