using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;

namespace FinClaude.Application.Features.Dashboards.Commands.DeleteDashboardChart;

public class DeleteDashboardChartCommandHandler(
    IChainProvider<DeleteDashboardChartContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<DeleteDashboardChartCommand>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteDashboardChartCommand command, CancellationToken ct = default)
    {
        var context = new DeleteDashboardChartContext
        {
            AccountId = command.AccountId,
            ChartId = command.ChartId,
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
        return Result.Success;
    }
}
